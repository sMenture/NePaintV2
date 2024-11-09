using NePaintV2.Windows;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.IO;
using System.Windows.Input;

namespace NePaintV2.Pages
{
    public partial class LaunchPage : Page
    {
        private SettingsNewProject settingsNewProject;
        private LaunchWindow launchWindow;
        private DrawingPage drawingPage;
        public LaunchPage(LaunchWindow launchWindow)
        {
            this.launchWindow = launchWindow;

            drawingPage = new DrawingPage(this, launchWindow);
            settingsNewProject = new SettingsNewProject(drawingPage, this);

            InitializeComponent();
            LoadProjectList();
        }


        private void CreateNewProject(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(settingsNewProject);
        }
        private void LoadProjectList()
        {
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NePaint");
            string projectListPath = Path.Combine(appDataFolder, "ProjectList.json");
            if (File.Exists(projectListPath))
            {
                string json = File.ReadAllText(projectListPath);
                List<ProjectInfo> projectInfos = JsonConvert.DeserializeObject<List<ProjectInfo>>(json);
                foreach (var projectInfo in projectInfos)
                {
                    StackPanel projectPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical, Margin = new Thickness(10)
                    };
                    Border projectBorder = new Border
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Wheat,
                        CornerRadius = new CornerRadius(10),
                        Margin = new Thickness(5),
                        Height = 150,
                        Width = 150,
                        Background = Brushes.Gray // Можно изменить цвет фона
                    };
                    TextBlock projectTitle = new TextBlock
                    {
                        Text = projectInfo.FileName,
                        Width = 100,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = Brushes.Wheat,
                        FontFamily = new FontFamily("Cascadia Code"),
                        FontStyle = FontStyles.Italic
                    };
                    TextBlock saveDate = new TextBlock
                    {
                        Text = projectInfo.SaveDate.ToString("MM/dd/yyyy"),
                        Width = 100,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = Brushes.Wheat,
                        FontFamily = new FontFamily("Cascadia Code"),
                        FontStyle = FontStyles.Italic
                    };

                    projectPanel.Children.Add(projectBorder);
                    projectPanel.Children.Add(projectTitle);
                    projectPanel.Children.Add(saveDate);
                    ProjectsStackPanel.Children.Add(projectPanel);

                    projectPanel.MouseDown += (sender, e) =>
                    {
                        try
                        {

                            drawingPage.LoadCanvas(projectInfo.FilePath);
                            NavigationService.Navigate(drawingPage);
                        }
                        catch { }
                    };
                }
            }
        }

    }
}

