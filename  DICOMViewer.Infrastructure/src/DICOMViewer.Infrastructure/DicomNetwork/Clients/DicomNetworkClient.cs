using FellowOakDicom;
using FellowOakDicom.Network;
using TheSSS.DICOMViewer.Infrastructure.DicomNetwork.Models;

namespace TheSSS.DICOMViewer.Infrastructure.DicomNetwork.Clients
{
    public class DicomNetworkClient : IDicomNetworkClient
    {
        private readonly ILoggerAdapter<DicomNetworkClient> _logger;

        public DicomNetworkClient(ILoggerAdapter<DicomNetworkClient> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendCEchoAsync(DicomNodeConfig nodeConfig)
        {
            try
            {
                using var client = new DicomClient();
                var cEchoRequest = new DicomCEchoRequest();
                
                await client.AddRequestAsync(cEchoRequest);
                await client.SendAsync(nodeConfig.Host, nodeConfig.Port, false, nodeConfig.AeTitle, nodeConfig.PeerAeTitle);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DICOM C-ECHO failed");
                return false;
            }
        }

        public async Task<CStoreResponse> SendCStoreAsync(DicomNodeConfig nodeConfig, IEnumerable<DicomFile> dicomFiles)
        {
            var response = new CStoreResponse();
            using var client = new DicomClient();
            
            foreach (var file in dicomFiles)
            {
                var cStoreRequest = new DicomCStoreRequest(file);
                await client.AddRequestAsync(cStoreRequest);
            }

            await client.SendAsync(nodeConfig.Host, nodeConfig.Port, false, nodeConfig.AeTitle, nodeConfig.PeerAeTitle);
            return response;
        }

        public void ConfigureLocalScp(string aeTitle, int port)
        {
            DicomServer.Create<DicomCStoreSCP>(aeTitle, port);
        }
    }
}