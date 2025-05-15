using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheSSS.DicomViewer.Application.Interfaces.Application;
using TheSSS.DicomViewer.Application.Services;

namespace TheSSS.DicomViewer.Application.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IDicomImportService, DicomImportOrchestratorService>();
        services.AddScoped<IAnonymizationOrchestrationService, AnonymizationOrchestrationService>();
        services.AddScoped<IDicomNetworkService, DicomNetworkOrchestratorService>();

        return services;
    }
}