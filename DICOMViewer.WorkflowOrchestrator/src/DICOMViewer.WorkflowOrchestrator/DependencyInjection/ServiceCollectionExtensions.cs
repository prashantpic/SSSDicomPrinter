using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Configuration;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Handlers;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Services;
using TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Validators;
using FluentValidation;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflowOrchestrator(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<WorkflowOrchestratorSettings>(configuration.GetSection("WorkflowOrchestrator"));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<StartDicomImportWorkflowCommandHandler>());
            
            services.AddScoped<IResourceGovernor, ResourceGovernorService>();
            services.AddSingleton<NetworkOperationCoordinator>();
            
            services.AddValidatorsFromAssemblyContaining<StartDicomImportWorkflowCommandValidator>();
            
            return services;
        }
    }
}