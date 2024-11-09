using NePaintV2.Class.DrawClass;
using NePaintV2.Pages;
using NePaintV2.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NePaintV2.Class.NePaint_v2
{
    public class KeyboardHandler
    {
        private DrawingCanvas drawingCanvas;
        private DrawingPage drawingPage;

        private LaunchWindow launchWindow;
        private System.Windows.Controls.TextBox shapeInfoLabel;

        public KeyboardHandler(DrawingCanvas drawingCanvas, Image image, DrawingPage drawingPage, System.Windows.Controls.TextBox shapeInfoLabel, LaunchWindow launchWindow)
        {
            this.drawingCanvas = drawingCanvas;
            this.launchWindow = launchWindow;
            this.drawingPage = drawingPage;
            this.drawingCanvas = drawingCanvas;
            this.shapeInfoLabel = shapeInfoLabel;

            launchWindow.KeyDown += HandleKeyDown;
            image.MouseWheel += CanvasImage_MouseWheel;
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && IsEraseMode)
            {
                drawingCanvas.RestoreState();
            }
        }
        private void CanvasImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (IsEraseMode)
            {
                int delta = e.Delta > 0 ? 1 : -1;
                drawingCanvas.ChangeBrushSize(delta);
                shapeInfoLabel.Text = $"{drawingCanvas.BrushSize}";

                drawingPage.UpdateBrushEllipseSize();
            }
        }

        public bool IsEraseMode => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
    }
}
