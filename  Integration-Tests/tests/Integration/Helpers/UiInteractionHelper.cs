using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input; // For ICommand

namespace TheSSS.DicomViewer.IntegrationTests.Helpers
{
    public static class UiInteractionHelper
    {
        public static async Task TriggerImageLoadAsync(object viewModel, string filePath)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            Type viewModelType = viewModel.GetType();

            // Try to find a command that takes a string (filePath)
            string[] commandNamesWithParam = { "LoadDicomFileCommand", "LoadFileCommand", "LoadImageCommand", "OpenDicomFileCommand" };
            foreach (var name in commandNamesWithParam)
            {
                var commandProperty = viewModelType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (commandProperty != null && typeof(ICommand).IsAssignableFrom(commandProperty.PropertyType))
                {
                    var command = (ICommand)commandProperty.GetValue(viewModel);
                    if (command != null && command.CanExecute(filePath))
                    {
                        var executeResult = command.Execute(filePath);
                        if (executeResult is Task task)
                        {
                            await task;
                        }
                        return;
                    }
                }

                // Also check for methods like LoadAsync(string filePath)
                var methodInfo = viewModelType.GetMethod(name, BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null) ??
                                 viewModelType.GetMethod(name + "Async", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null);

                if (methodInfo != null)
                {
                    var result = methodInfo.Invoke(viewModel, new object[] { filePath });
                    if (result is Task task)
                    {
                        await task;
                    }
                    return;
                }
            }

            // Try to find a FilePath property and a parameterless command
            var filePathProperty = viewModelType.GetProperty("FilePath", BindingFlags.Public | BindingFlags.Instance);
            if (filePathProperty != null && filePathProperty.CanWrite && filePathProperty.PropertyType == typeof(string))
            {
                filePathProperty.SetValue(viewModel, filePath);

                string[] parameterlessCommandNames = { "LoadCommand", "LoadDicomCommand", "LoadImageCommand", "ExecuteLoadCommand" };
                foreach (var name in parameterlessCommandNames)
                {
                    var commandProperty = viewModelType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                    if (commandProperty != null && typeof(ICommand).IsAssignableFrom(commandProperty.PropertyType))
                    {
                        var command = (ICommand)commandProperty.GetValue(viewModel);
                        if (command != null && command.CanExecute(null)) // Parameterless command
                        {
                            var executeResult = command.Execute(null);
                            if (executeResult is Task task)
                            {
                                await task;
                            }
                            return;
                        }
                    }
                }
            }
            
            throw new InvalidOperationException(
                $"Could not find a suitable command or method on ViewModel '{viewModelType.Name}' to load file '{filePath}'. " +
                $"Searched for commands like LoadDicomFileCommand(string), or FilePath property + LoadCommand().");
        }
    }
}