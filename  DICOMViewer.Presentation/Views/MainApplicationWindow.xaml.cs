using System.Windows;
using TheSSS.DICOMViewer.Presentation.ViewModels;

namespace TheSSS.DICOMViewer.Presentation.Views
{
    public partial class MainApplicationWindow : Window
    {
        public MainApplicationWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<MainApplicationWindowViewModel>();
        }
    }
}