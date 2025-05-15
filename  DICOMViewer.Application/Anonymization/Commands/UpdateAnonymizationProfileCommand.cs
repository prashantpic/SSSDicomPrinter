using MediatR;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands;

public record UpdateAnonymizationProfileCommand(
    string ProfileId,
    string ProfileName,
    string ProfileDescription,
    List<MetadataRuleDto> MetadataRules
) : IRequest<AnonymizationProfileDto>;