using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TheSSS.DICOMViewer.Infrastructure.Persistence.DbContext;
using TheSSS.DICOMViewer.Infrastructure.Persistence.Repositories;
using TheSSS.DICOMViewer.Application.Interfaces.Persistence;

namespace TheSSS.DICOMViewer.Infrastructure.Configuration.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<DicomDbContext>(options => 
                options.UseSqlite(connectionString));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IDicomNetworkClient, DicomNetworkClient>();
            services.AddScoped<IDicomImageRenderer, DicomImageRenderingService>();
            services.AddScoped<IPdfGenerator, PdfGenerationService>();
            services.AddSingleton<ISecureConfigurationHandler, SecureConfigurationHandler>();

            return services;
        }
    }
}