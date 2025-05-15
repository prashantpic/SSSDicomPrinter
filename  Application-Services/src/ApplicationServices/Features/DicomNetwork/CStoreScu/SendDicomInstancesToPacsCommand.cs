using MediatR;
using TheSSS.DicomViewer.Application.DTOs.Pacs;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CStoreScu;

public record SendDicomInstancesToPacsCommand(int PacsNodeId, IEnumerable<string> SopInstanceUids) 
    : IRequest<CStoreScuResultDto>;