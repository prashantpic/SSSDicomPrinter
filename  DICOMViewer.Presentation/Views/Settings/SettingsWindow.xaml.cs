using System.Windows;

namespace TheSSS.DICOMViewer.Presentation.Views.Settings
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<ViewModels.Settings.SettingsWindowViewModel>();
        }
    }
}