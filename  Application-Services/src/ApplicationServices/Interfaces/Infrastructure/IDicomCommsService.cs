using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Infrastructure
{
    public interface IDicomCommsService
    {
        Task<CEchoResultDto> SendCEchoAsync(PacsConfigurationDto pacsConfig, CancellationToken cancellationToken);
        Task<CStoreScuResultDto> SendCStoreAsync(PacsConfigurationDto pacsConfig, IEnumerable<string> filePaths, CancellationToken cancellationToken);
    }
}