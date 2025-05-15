using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

namespace TheSSS.DICOMViewer.Application.Common.Behaviours
{
    public class AuditingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IAuditLogRepositoryAdapter _auditLogRepository;

        public AuditingBehaviour(IAuditLogRepositoryAdapter auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();
            
            if (request is IBaseRequest)
            {
                await _auditLogRepository.LogEventAsync(
                    eventType: request.GetType().Name,
                    eventDetails: "Command executed successfully",
                    outcome: "Success");
            }
            
            return response;
        }
    }
}