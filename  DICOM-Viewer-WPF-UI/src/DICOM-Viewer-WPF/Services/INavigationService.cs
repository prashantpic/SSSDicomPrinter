namespace TheSSS.DicomViewer.Presentation.Services
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>() where TViewModel : class;
        void NavigateTo<TViewModel>(object parameter) where TViewModel : class;
        void GoBack();
        bool CanGoBack { get; }
    }
}