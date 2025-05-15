using MediatR;
using TheSSS.DICOMViewer.Application.DicomNetwork.DTOs;
using TheSSS.DICOMViewer.Infrastructure.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.DicomNetwork.Commands.Handlers
{
    public class VerifyPacsConnectivityCommandHandler : IRequestHandler<VerifyPacsConnectivityCommand, DicomNetworkResultDto>
    {
        private readonly ISettingsRepositoryAdapter _settingsRepository;
        private readonly IDicomNetworkClientAdapter _networkClient;
        private readonly IAuditLogRepositoryAdapter _auditLog;

        public VerifyPacsConnectivityCommandHandler(
            ISettingsRepositoryAdapter settingsRepository,
            IDicomNetworkClientAdapter networkClient,
            IAuditLogRepositoryAdapter auditLog)
        {
            _settingsRepository = settingsRepository;
            _networkClient = networkClient;
            _auditLog = auditLog;
        }

        public async Task<DicomNetworkResultDto> Handle(VerifyPacsConnectivityCommand request, CancellationToken cancellationToken)
        {
            var pacsConfig = await _settingsRepository.GetPacsConfigurationByIdAsync(request.PacsNodeId)
                ?? throw new NotFoundException($"PACS configuration with ID {request.PacsNodeId} not found");

            try
            {
                var result = await _networkClient.SendCEchoAsync(pacsConfig);
                
                await _settingsRepository.UpdatePacsConnectionStatusAsync(
                    request.PacsNodeId,
                    DateTime.UtcNow,
                    result.Success ? "Success" : "Failed");

                await _auditLog.LogEventAsync("PacsConnectionTest", 
                    $"C-ECHO to {pacsConfig.AeTitle} ({pacsConfig.Hostname}:{pacsConfig.Port})",
                    result.Success ? "Success" : "Failed");

                return new DicomNetworkResultDto(
                    result