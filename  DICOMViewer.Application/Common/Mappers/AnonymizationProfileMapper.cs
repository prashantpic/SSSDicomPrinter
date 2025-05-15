using AutoMapper;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;
using TheSSS.DICOMViewer.Domain.Entities;

namespace TheSSS.DICOMViewer.Application.Common.Mappers;

public class AnonymizationProfileMapper : Profile
{
    public AnonymizationProfileMapper()
    {
        CreateMap<AnonymizationProfile, AnonymizationProfileDto>().ReverseMap();
        CreateMap<MetadataRule, MetadataRuleDto>().ReverseMap();
        CreateMap<PixelAnonymizationTemplate, PixelAnonymizationTemplateDto>().ReverseMap();
    }
}