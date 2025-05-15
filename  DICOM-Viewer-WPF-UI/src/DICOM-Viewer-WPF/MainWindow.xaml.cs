using TheSSS.DicomViewer.Presentation.ViewModels;

namespace TheSSS.DicomViewer.Presentation;

public partial class MainWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}