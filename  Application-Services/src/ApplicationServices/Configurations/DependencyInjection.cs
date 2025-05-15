using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using MediatR;
using AutoMapper;

namespace TheSSS.DicomViewer.Application.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            services.AddScoped<IDicomImportService, DicomImportOrchestratorService>();
            services.AddScoped<IAnonymizationOrchestrationService, AnonymizationOrchestrationService>();
            services.AddScoped<IDicomNetworkService, DicomNetworkOrchestratorService>();

            return services;
        }
    }
}