using NePaintV2.Pages;
using System.Windows;
using System.Windows.Input;

namespace NePaintV2.Windows
{
    public partial class LaunchWindow : Window
    {
        private LaunchPage launchPage;
        public LaunchWindow()
        {
            InitializeComponent();

            launchPage = new LaunchPage(this);

            mainFrame.Navigate(launchPage);
        }
    }
}
