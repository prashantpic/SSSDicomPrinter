using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Infrastructure
{
    public interface IDicomCommsService
    {
        Task<bool> SendCEchoAsync(string aeTitle, string host, int port, CancellationToken cancellationToken = default);
        Task<CStoreScuResultDto> SendCStoreAsync(string aeTitle, string host, int port, IEnumerable<string> filePaths, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Models.DicomStudy>> QueryStudiesAsync(string aeTitle, string host, int port, Domain.Models.StudyQuery query, CancellationToken cancellationToken = default);
    }
}