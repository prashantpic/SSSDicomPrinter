using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Presentation.Services
{
    public interface IUserDialogService
    {
        Task ShowMessageAsync(string title, string message);
        Task<bool> ShowConfirmationAsync(string title, string message);
        Task<bool?> ShowDialogAsync(object viewModel);
    }
}