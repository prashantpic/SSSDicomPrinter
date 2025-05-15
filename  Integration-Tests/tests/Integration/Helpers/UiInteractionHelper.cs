using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input; // For ICommand

namespace TheSSS.DicomViewer.IntegrationTests.Helpers
{
    public class UiInteractionHelper
    {
        private readonly IServiceProvider _serviceProvider;

        public UiInteractionHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Finds and triggers a command on a ViewModel.
        /// This method attempts to run the command on the UI thread if a dispatcher is available and configured,
        /// otherwise executes it directly. For integration tests, direct execution is often preferred.
        /// </summary>
        /// <param name="viewModel">The ViewModel instance.</param>
        /// <param name="commandPropertyName">The name of the ICommand property on the ViewModel (e.g., "LoadDicomFileCommand").</param>
        /// <param name="commandParameter">The parameter to pass to the command (e.g., file path).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task TriggerCommandAsync(object viewModel, string commandPropertyName, object? commandParameter = null)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            if (string.IsNullOrWhiteSpace(commandPropertyName)) throw new ArgumentNullException(nameof(commandPropertyName));

            var commandProperty = viewModel.GetType().GetProperty(commandPropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (commandProperty == null)
            {
                throw new MissingMemberException($"ViewModel of type {viewModel.GetType().Name} does not have a public instance property named '{commandPropertyName}'.");
            }

            if (commandProperty.GetValue(viewModel) is not ICommand command)
            {
                throw new InvalidCastException($"Property '{commandPropertyName}' on ViewModel of type {viewModel.GetType().Name} is not an ICommand.");
            }

            if (!command.CanExecute(commandParameter))
            {
                throw new InvalidOperationException($"Command '{commandPropertyName}' on ViewModel of type {viewModel.GetType().Name} cannot be executed with the provided parameter (CanExecute returned false).");
            }

            // Execute the command. Handle both synchronous and asynchronous commands.
            // For WPF, commands are typically executed on the UI thread.
            // In integration tests, direct execution might be fine if the command logic itself
            // doesn't have hard UI thread dependencies not handled by the ViewModel.
            // If a dispatcher is strictly needed, it would have to be injected or accessed globally (less ideal).

            var executionResult = command.Execute(commandParameter);

            // If the command.Execute() itself returns a Task (e.g., for AsyncRelayCommand from MVVM toolkits),
            // await its completion.
            if (executionResult is Task task)
            {
                await task;
            }
            // Some ICommand implementations might not return Task but still perform async operations internally.
            // For those, ensure the test awaits a separate signal of completion if needed (e.g., ViewModel property change).
        }

        /// <summary>
        /// Triggers the image load command on a ViewModel, simulating user file selection.
        /// </summary>
        /// <param name="viewModel">The ViewModel instance (e.g., DicomImageViewerViewModel).</param>
        /// <param name="filePath">The path to the DICOM file to load.</param>
        /// <param name="commandName">The name of the command property on the ViewModel. Defaults to "LoadDicomFileCommand".</param>
        public async Task TriggerImageLoadAsync(object viewModel, string filePath, string commandName = "LoadDicomFileCommand")
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            await TriggerCommandAsync(viewModel, commandName, filePath);
        }

        // Example of a helper for getting a ViewModel (if needed beyond AppHostFixture's GetService)
        // public TViewModel GetViewModel<TViewModel>() where TViewModel : class
        // {
        //     return _serviceProvider.GetRequiredService<TViewModel>();
        // }
    }
}