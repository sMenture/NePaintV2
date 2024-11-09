using NePaintV2.Class.DrawClass;
using NePaintV2.Class.NePaint_v2;
using NePaintV2.Windows;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NePaintV2.Pages
{
    public partial class DrawingPage : Page
    {
        private LaunchPage launchPage;

        private SaveLogic saveLogic = new SaveLogic();
        SaveProject project = new SaveProject();
        private SwitchColor switchColor = new SwitchColor();

        private LaunchWindow launchWindow;
        private DrawingCanvas drawingCanvas;
        private KeyboardHandler keyboardHandler;

        public DrawingPage(LaunchPage launchPage, LaunchWindow launchWindow)
        {
            this.launchWindow = launchWindow;
            this.launchPage = launchPage;

            InitializeComponent();
        }
        public void CreateNewCanvas(int hight, int width, string name, Color color)
        {
            this.Title = $"Solution '{name}'";

            drawingCanvas = new DrawingCanvas(width, hight, color);
            canvasImage.Source = drawingCanvas.Bitmap;


            InitializeCanvas();
        }

        private async void InitializeCanvas()
        {
            Color colorStart = Colors.Transparent;
            if (selectedColorDisplay.Fill is SolidColorBrush solidColorBrush)
            {
                colorStart = solidColorBrush.Color;
            }
            drawingCanvas.SetSelectedColor(colorStart);

            await System.Threading.Tasks.Task.Delay(100);
            keyboardHandler = new KeyboardHandler(drawingCanvas, canvasImage, this, shapeInfoLabel, launchWindow);

            shapeInfoLabel.Text = $"{drawingCanvas.BrushSize}";
            canvasImage.MouseMove += CanvasImage_MouseMove;


            shapeInfoLabel.TextChanged += ShapeInfoLabel_TextChanged;
            selectedColorDisplay.MouseDown += SelectedColorDisplay_MouseDown;

            canvasImage.MouseLeave += (object sender, MouseEventArgs e) =>
            {
                brushEllipse.Visibility = Visibility.Collapsed;
                Mouse.OverrideCursor = null;
            };
            canvasImage.MouseEnter += (object sender, MouseEventArgs e) =>
            {
                brushEllipse.Visibility = Visibility.Visible;
                Mouse.OverrideCursor = Cursors.None;
            };

            await System.Threading.Tasks.Task.Delay(100);
            UpdateBrushEllipsePosition();
            UpdateBrushEllipseSize();
        }



        public void UpdateBrushEllipsePosition()
        {
            var position = Mouse.GetPosition(canvasImage);
            var scaleX = canvasImage.ActualWidth / drawingCanvas.Bitmap.PixelWidth;
            var scaleY = canvasImage.ActualHeight / drawingCanvas.Bitmap.PixelHeight;
            var adjustedPosition = new Point(position.X / scaleX, position.Y / scaleY);

            Canvas.SetLeft(brushEllipse, position.X - brushEllipse.Width / 2);
            Canvas.SetTop(brushEllipse, position.Y - brushEllipse.Height / 2);
        }
        public void UpdateBrushEllipseSize()
        {
            var scaleX = canvasImage.ActualWidth / drawingCanvas.Bitmap.PixelWidth;
            var scaleY = canvasImage.ActualHeight / drawingCanvas.Bitmap.PixelHeight;
            var brushSizeScale = (scaleX + scaleY) / 2;
            brushEllipse.Width = drawingCanvas.BrushSize * brushSizeScale;
            brushEllipse.Height = drawingCanvas.BrushSize * brushSizeScale;
        }


        private void CanvasImage_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(canvasImage);
            var scaleX = canvasImage.ActualWidth / drawingCanvas.Bitmap.PixelWidth;
            var scaleY = canvasImage.ActualHeight / drawingCanvas.Bitmap.PixelHeight;
            var adjustedPosition = new Point(position.X / scaleX, position.Y / scaleY);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                drawingCanvas.SaveState();
                drawingCanvas.HandleMouseMove(adjustedPosition, keyboardHandler.IsEraseMode);
            }
            else
                drawingCanvas.previousPoint = null;

            UpdateBrushEllipsePosition();
        }

        private void SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SaveProject saveProject = new SaveProject();
            saveProject.SaveProjectData(this.Title, (int)canvasImage.Width, (int)canvasImage.Height);
        }




        private void LoadProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "NePaint Project Files (*.npproj)|*.npproj";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadCanvas(openFileDialog.FileName);
            }
        }
        public void LoadCanvas(string filePath)
        {
            SaveProject saveProject = new SaveProject();
            ProjectData projectData = saveProject.LoadProject(filePath);

            this.Title = projectData.ProjectName;

            drawingCanvas = new DrawingCanvas(projectData.CanvasWidth, projectData.CanvasHeight, Colors.White);
            canvasImage.Source = drawingCanvas.Bitmap;

            InitializeCanvas();
        }





        private void ShapeInfoLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(shapeInfoLabel.Text, out int newSize))
            {
                drawingCanvas.ChangeBrushSize(newSize - drawingCanvas.BrushSize);
                UpdateBrushEllipseSize();
            }
        }
        private void SelectedColorDisplay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            drawingCanvas.SetSelectedColor(switchColor.Switch(selectedColorDisplay));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) => saveLogic.Save(drawingCanvas);
        private void GoBack_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(launchPage);

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); e.Handled = regex.IsMatch(e.Text);
        }
    }
}
