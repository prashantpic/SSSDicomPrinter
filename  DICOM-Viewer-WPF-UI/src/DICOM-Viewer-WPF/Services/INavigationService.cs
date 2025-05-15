namespace TheSSS.DicomViewer.Presentation.Services
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>() where TViewModel : class;
    }
}