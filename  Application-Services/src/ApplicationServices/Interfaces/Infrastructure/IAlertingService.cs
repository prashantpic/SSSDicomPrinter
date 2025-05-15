using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Infrastructure
{
    public interface IAlertingService
    {
        Task SendAlertAsync(string alertType, string message, string details, CancellationToken cancellationToken);
    }
}