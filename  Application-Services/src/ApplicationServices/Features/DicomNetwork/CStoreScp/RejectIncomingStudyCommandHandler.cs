using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CStoreScp
{
    public class RejectIncomingStudyCommandHandler : IRequestHandler<RejectIncomingStudyCommand, Unit>
    {
        private readonly IFileSystemService _fileSystemService;

        public RejectIncomingStudyCommandHandler(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        public async Task<Unit> Handle(RejectIncomingStudyCommand request, CancellationToken cancellationToken)
        {
            await _fileSystemService.MoveToRejectedArchiveAsync(request.StudyInstanceUidOrPath);
            return Unit.Value;
        }
    }
}