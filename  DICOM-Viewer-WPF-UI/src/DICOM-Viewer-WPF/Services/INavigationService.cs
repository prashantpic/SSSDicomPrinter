namespace TheSSS.DicomViewer.Presentation.Services
{
    public interface INavigationService
    {
        void NavigateTo(Type viewModelType);
        void NavigateTo(Type viewModelType, object parameter);
        void GoBack();
    }
}