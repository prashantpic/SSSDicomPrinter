using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using foDICOM.Network;
using TheSSS.DICOMViewer.Application.Interfaces.Networking;
using TheSSS.DICOMViewer.Infrastructure.DicomNetwork.Models;

namespace TheSSS.DICOMViewer.Infrastructure.DicomNetwork.Clients
{
    public class DicomNetworkClient : IDicomNetworkClient
    {
        public async Task<bool> SendCEchoAsync(DicomNodeConfig nodeConfig, CancellationToken cancellationToken)
        {
            using var client = new DICOMClient(nodeConfig.Host, nodeConfig.Port, nodeConfig.AETitle, "SCU");
            var request = new DICOMRequest(DICOMCommand.CECHO);
            var response = await client.SendRequestAsync(request, cancellationToken);
            return response.Status == DICOMStatus.Success;
        }

        public async Task<CStoreResponse> SendCStoreAsync(DicomNodeConfig nodeConfig, IEnumerable<DicomFile> dicomFiles, CancellationToken cancellationToken)
        {
            var response = new CStoreResponse();
            using var client = new DICOMClient(nodeConfig.Host, nodeConfig.Port, nodeConfig.AETitle, "SCU");

            foreach (var file in dicomFiles)
            {
                var request = new DICOMStoreRequest(file);
                var storeResponse = await client.SendRequestAsync(request, cancellationToken);
                response.AddResult(file.SOPInstanceUID, storeResponse.Status == DICOMStatus.Success);
            }

            return response;
        }

        public void ConfigureLocalScp(string aeTitle, int port, string storagePath)
        {
            var server = new DICOMServer(port, aeTitle);
            server.AddService(new DICOMStorageService(storagePath));
        }

        // Implement remaining interface methods similarly
    }
}