using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.DicomNetwork.DTOs;

namespace TheSSS.DICOMViewer.Application.DicomNetwork.Interfaces
{
    public interface IDicomNetworkService
    {
        Task<DicomNetworkResultDto> VerifyPacsConnectivityAsync(string pacsNodeId);
    }
}