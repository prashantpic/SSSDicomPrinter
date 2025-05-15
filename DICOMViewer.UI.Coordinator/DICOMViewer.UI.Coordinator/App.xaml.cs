using Prism.Unity;
using System.Windows;

namespace TheSSS.DICOMViewer.Presentation.Coordinator
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}