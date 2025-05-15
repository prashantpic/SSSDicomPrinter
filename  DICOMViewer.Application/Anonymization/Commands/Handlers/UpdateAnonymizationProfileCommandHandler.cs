using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Anonymization.Commands;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;
using TheSSS.DICOMViewer.Application.Common.Exceptions;
using TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands.Handlers;

public class UpdateAnonymizationProfileCommandHandler : IRequestHandler<UpdateAnonymizationProfileCommand, AnonymizationProfileDto>
{
    private readonly ISettingsRepositoryAdapter _settingsRepository;
    private readonly IAuditLogRepositoryAdapter _auditLogger;

    public UpdateAnonymizationProfileCommandHandler(
        ISettingsRepositoryAdapter settingsRepository,
        IAuditLogRepositoryAdapter auditLogger)
    {
        _settingsRepository = settingsRepository;
        _auditLogger = auditLogger;
    }

    public async Task<AnonymizationProfileDto> Handle(UpdateAnonymizationProfileCommand request, CancellationToken cancellationToken)
    {
        var existingProfile = await _settingsRepository.GetAnonymizationProfileByIdAsync(request.ProfileId)
            ?? throw new NotFoundException(nameof(AnonymizationProfileDto), request.ProfileId);

        if