using NePaintV2.Class.DrawClass;
using NePaintV2.Windows;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace NePaintV2.Pages
{
    public partial class SettingsNewProject : Page
    {
        private LaunchPage launchPage;
        private DrawingPage drawingPage;
        private SwitchColor switchColor;
        public SettingsNewProject(DrawingPage page, LaunchPage launchPage)
        {
            drawingPage = page;
            this.launchPage = launchPage;
            switchColor = new SwitchColor();

            InitializeComponent();
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            string nameProject = ProjectName.Text;
            int width = int.Parse(ProjectWidth.Text);
            int height = int.Parse(ProjectHeight.Text);

            Color color = Colors.Transparent;
            if (ProjectColor.Fill is SolidColorBrush solidColorBrush)
            {
                color = solidColorBrush.Color;
            }

            drawingPage.CreateNewCanvas(width, height, nameProject, color);

            NavigationService.Navigate(drawingPage);
        }
        private void BackToLaunch(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(launchPage);
        }
        private void ChangeColor_Click(object sender, RoutedEventArgs e)
        {
            switchColor.Switch(ProjectColor);
        }


        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); e.Handled = regex.IsMatch(e.Text);
        }
    }
}
