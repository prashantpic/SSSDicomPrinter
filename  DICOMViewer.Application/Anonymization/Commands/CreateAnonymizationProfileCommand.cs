using MediatR;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands;

public record CreateAnonymizationProfileCommand(
    string ProfileName,
    string ProfileDescription,
    List<MetadataRuleDto> MetadataRules,
    int? PixelAnonymizationTemplateId
) : IRequest<AnonymizationProfileDto>;