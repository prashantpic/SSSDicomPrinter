using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Infrastructure
{
    public interface IAlertingService
    {
        Task SendAlertAsync(string message, AlertLevel level, CancellationToken cancellationToken = default);
        Task SendCriticalAlertAsync(string component, string message, CancellationToken cancellationToken = default);
    }

    public enum AlertLevel
    {
        Information,
        Warning,
        Critical
    }
}