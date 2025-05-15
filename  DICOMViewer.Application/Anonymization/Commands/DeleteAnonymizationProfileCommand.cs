using MediatR;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands;

public record DeleteAnonymizationProfileCommand(
    string ProfileId
) : IRequest<bool>;