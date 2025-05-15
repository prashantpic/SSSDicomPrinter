using AutoMapper;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;

namespace TheSSS.DICOMViewer.Application.Common.Mappers
{
    public class AnonymizationProfileMapper : Profile
    {
        public AnonymizationProfileMapper()
        {
            CreateMap<AnonymizationProfileDto, Domain.AnonymizationProfile>()
                .ReverseMap();
        }
    }
}