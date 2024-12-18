﻿using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;

namespace NePaintV2.Class.DrawClass
{
    public class DrawingCanvas
    {
        private bool showOutOfBoundsWarning = true;

        private WriteableBitmap writeableBitmap;
        public WriteableBitmap Bitmap => writeableBitmap;
        public ActionHistory actionHistory = new ActionHistory();

        private int width;
        private int height;
        public Point? previousPoint = null;
        public Color selectedColor = Colors.Black;
        private Color startColor;
        public int BrushSize { get; private set; } = 30;
        public double BrushHardness { get; set; } = 10;


        public DrawingCanvas(int width, int height, Color startColor)
        {
            this.startColor = startColor;

            this.width = width;
            this.height = height;
            BrushSize = Math.Min(width, height) / 10;
            InitializeCanvas();
        }




        private void InitializeCanvas()
        {
            writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            ClearCanvas();
        }
        public void SetBitmap(WriteableBitmap bitmap)
        {
            writeableBitmap = bitmap;
            // Перерисовываем холст
            writeableBitmap.Lock();
            Int32Rect rect = new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight);
            writeableBitmap.AddDirtyRect(rect);
            writeableBitmap.Unlock();
        }


        public void ClearCanvas()
        {
            writeableBitmap.Lock();

            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    IntPtr pBackBuffer = writeableBitmap.BackBuffer + y * writeableBitmap.BackBufferStride;
                    for (int x = 0; x < width; x++)
                    {
                        *((int*)pBackBuffer + x) = (startColor.A << 24) | (startColor.R << 16) | (startColor.G << 8) | startColor.B;
                    }
                }
            }

            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            writeableBitmap.Unlock();
        }

        public void HandleMouseMove(Point position, bool eraseMode)
        {
            var currentPoint = new Point((int)position.X, (int)position.Y);

            if (previousPoint != null)
            {
                DrawLine((Point)previousPoint, currentPoint, eraseMode);
            }
            else
            {
                DrawPixel((int)currentPoint.X, (int)currentPoint.Y, eraseMode);
            }
            previousPoint = currentPoint;
        }


        public void SaveState()
        {
            byte[] pixelData = new byte[writeableBitmap.PixelWidth * writeableBitmap.PixelHeight * 4];
            writeableBitmap.CopyPixels(pixelData, writeableBitmap.BackBufferStride, 0);
            actionHistory.AddAction(pixelData);
        }
        public void RestoreState()
        {
            byte[] previousState = actionHistory.Undo();
            if (previousState != null)
            {
                writeableBitmap.WritePixels(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight), previousState, writeableBitmap.BackBufferStride, 0);
            }
        }


        private void DrawPixel(int x, int y, bool eraseMode)
        {
            int limit = BrushSize / 2;
            if (x < 0 + limit || x + limit > width || y < 0 + limit || y + limit > height)
            {
                if (showOutOfBoundsWarning)
                {
                    MessageBoxResult result;
                    result = MessageBox.Show("" +
                        "Кисть вышла за пределы холста!\n\n" +
                        "Данное сообщение будет высвечиваться каждый раз, когда ваш курсор будет покидать пределы холста.", "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Cancel)
                        showOutOfBoundsWarning = false;
                }
                return;
            }

            try
            {
                Color baseColor = eraseMode ? startColor : selectedColor;
                writeableBitmap.Lock();

                unsafe
                {
                    IntPtr pBackBuffer = writeableBitmap.BackBuffer;
                    pBackBuffer += y * writeableBitmap.BackBufferStride;
                    pBackBuffer += x * 4;

                    byte* pPixels = (byte*)writeableBitmap.BackBuffer.ToPointer();

                    for (int i = -BrushSize / 2; i < BrushSize / 2; i++)
                    {
                        for (int j = -BrushSize / 2; j < BrushSize / 2; j++)
                        {
                            int nx = x + i;
                            int ny = y + j;
                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                double distance = Math.Sqrt(i * i + j * j);
                                if (distance > BrushSize / 2)
                                    continue;

                                double hardnessFactor = BrushHardness / 100.0;
                                double opacity = Math.Max(0, hardnessFactor - (distance / (BrushSize / 2)) * hardnessFactor);
                                int alpha = (int)(baseColor.A * opacity);

                                int index = (ny * writeableBitmap.BackBufferStride) + (nx * 4);
                                byte origAlpha = pPixels[index + 3];
                                byte origRed = pPixels[index + 2];
                                byte origGreen = pPixels[index + 1];
                                byte origBlue = pPixels[index];

                                byte newRed = (byte)((baseColor.R * alpha + origRed * (255 - alpha)) / 255);
                                byte newGreen = (byte)((baseColor.G * alpha + origGreen * (255 - alpha)) / 255);
                                byte newBlue = (byte)((baseColor.B * alpha + origBlue * (255 - alpha)) / 255);
                                byte newAlpha = (byte)Math.Min(255, origAlpha + alpha);

                                pPixels[index] = newBlue;
                                pPixels[index + 1] = newGreen;
                                pPixels[index + 2] = newRed;
                                pPixels[index + 3] = newAlpha;
                            }
                        }
                    }
                }

                writeableBitmap.AddDirtyRect(new Int32Rect(x - BrushSize / 2, y - BrushSize / 2, BrushSize, BrushSize));
                writeableBitmap.Unlock();
            }
            catch
            {
                writeableBitmap.Unlock();
            }
        }


        private void DrawLine(Point from, Point to, bool eraseMode)
        {
            int x0 = (int)from.X;
            int y0 = (int)from.Y;
            int x1 = (int)to.X;
            int y1 = (int)to.Y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            writeableBitmap.Lock();
            while (true)
            {
                DrawPixel(x0, y0, eraseMode);

                if (x0 == x1 && y0 == y1) break;
                int e2 = err * 2;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
            writeableBitmap.Unlock();
        }





        public void SetSelectedColor(Color color)
        {
            selectedColor = color;
        }
        public void ChangeBrushSize(int delta)
        {
            BrushSize = BrushSize + delta;
            if (BrushSize < 1)
                BrushSize = 1;
            else if (BrushSize > 500)
                BrushSize = 500;

        }
        public void ChangeBrushHardness(int delta)
        {
            BrushHardness = BrushHardness + delta;
            if (BrushHardness < 1)
                BrushHardness = 1;
            else if (BrushHardness > 500)
                BrushHardness = 500;

        }

        public List<byte[]> GetHistory()
        {
            return actionHistory.GetAllActions();
        }
        public void SetHistory(List<byte[]> history)
        {
            actionHistory.SetActions(history);
        }
    }
}
