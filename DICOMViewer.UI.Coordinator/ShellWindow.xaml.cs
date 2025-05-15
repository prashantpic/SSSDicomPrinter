using System.Windows;
using DICOMViewer.UI.Coordinator.ViewModels;

namespace DICOMViewer.UI.Coordinator
{
    public partial class ShellWindow : Window
    {
        public ShellWindow(ShellViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}