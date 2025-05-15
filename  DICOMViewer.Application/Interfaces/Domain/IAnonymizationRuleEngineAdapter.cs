using System.IO;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;

namespace TheSSS.DICOMViewer.Application.Interfaces.Domain;

public interface IAnonymizationRuleEngineAdapter
{
    Task<Stream> ApplyMetadataAnonymizationAsync(Stream dicomStream, AnonymizationProfileDto profile);
}