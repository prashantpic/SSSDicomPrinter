using MediatR;
using System.Collections.Generic;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands;

public record CreateAnonymizationProfileCommand(
    string ProfileName,
    string ProfileDescription,
    List<MetadataRuleDto> MetadataRules,
    string? PredefinedRuleSetName,
    int? PixelAnonymizationTemplateId,
    bool IsReadOnly) : IRequest<AnonymizationProfileDto>;