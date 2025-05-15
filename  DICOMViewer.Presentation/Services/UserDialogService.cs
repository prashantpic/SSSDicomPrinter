using System.Windows;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Presentation.Services
{
    public class UserDialogService : IUserDialogService
    {
        private readonly IViewLocator _viewLocator;

        public UserDialogService(IViewLocator viewLocator)
        {
            _viewLocator = viewLocator;
        }

        public Task ShowMessageAsync(string title, string message)
        {
            MessageBox.Show(message, title);
            return Task.CompletedTask;
        }

        public Task<bool> ShowConfirmationAsync(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo);
            return Task.FromResult(result == MessageBoxResult.Yes);
        }

        public async Task<bool?> ShowDialogAsync(object viewModel)
        {
            var viewType = _viewLocator.ResolveViewType(viewModel.GetType());
            if (viewType == null) return null;

            var window = (Window)Activator.CreateInstance(viewType)!;
            window.DataContext = viewModel;
            return window.ShowDialog();
        }
    }
}