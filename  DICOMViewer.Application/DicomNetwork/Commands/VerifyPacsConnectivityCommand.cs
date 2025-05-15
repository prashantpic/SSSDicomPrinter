using MediatR;
using TheSSS.DICOMViewer.Application.DicomNetwork.DTOs;

namespace TheSSS.DICOMViewer.Application.DicomNetwork.Commands;

public record VerifyPacsConnectivityCommand(
    string PacsNodeId) : IRequest<DicomNetworkResultDto>;