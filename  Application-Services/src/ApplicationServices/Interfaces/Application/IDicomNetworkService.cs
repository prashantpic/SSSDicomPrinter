using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Application.DTOs.Pacs;

namespace TheSSS.DicomViewer.Application.Interfaces.Application
{
    public interface IDicomNetworkService
    {
        Task<CEchoResultDto> VerifyPacsConnectionAsync(int pacsNodeId, CancellationToken cancellationToken = default);
        Task<CStoreScuResultDto> SendInstancesToPacsAsync(int pacsNodeId, IEnumerable<string> sopInstanceUids, CancellationToken cancellationToken = default);
    }
}