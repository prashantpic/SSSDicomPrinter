using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

namespace TheSSS.DICOMViewer.Application.Common.Behaviours;

public class AuditingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IAuditLogRepositoryAdapter _auditLogRepository;

    public AuditingBehaviour(IAuditLogRepositoryAdapter auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!typeof(TRequest).Name.EndsWith("Command"))
            return await next();

        var eventType = typeof(TRequest).Name;
        var outcome = "Success";

        try
        {
            var response = await next();
            await _auditLogRepository.LogEventAsync(eventType, "Operation succeeded", outcome);
            return response;
        }
        catch (System.Exception ex)
        {
            outcome = "Failure";
            await _auditLogRepository.LogEventAsync(eventType, $"Error: {ex.Message}", outcome);
            throw;
        }
    }
}