using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Application.Anonymization.DTOs
{
    public record AnonymizationProfileDto(
        string ProfileId,
        string ProfileName,
        string ProfileDescription,
        List<MetadataRuleDto> MetadataRules,
        string PredefinedRuleSetName,
        int? PixelAnonymizationTemplateId,
        bool IsReadOnly
    );
}