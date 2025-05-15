using System.Collections.Generic;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.DicomNetwork.DTOs;

namespace TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

public interface IDicomNetworkClientAdapter
{
    Task<DicomNetworkResultDto> SendCEchoAsync(PacsConfigurationDto pacsConfig);
    Task<DicomNetworkResultDto> SendCStoreAsync(PacsConfigurationDto pacsConfig, IEnumerable<string> dicomFilePaths);
    Task<DicomQueryResultDto> SendCFindAsync(PacsConfigurationDto pacsConfig, DicomQueryParametersDto queryParameters);
    Task<DicomNetworkResultDto> SendCMoveAsync(PacsConfigurationDto pacsConfig, IEnumerable<string> studyInstanceUidsToRetrieve, string destinationAet);
}