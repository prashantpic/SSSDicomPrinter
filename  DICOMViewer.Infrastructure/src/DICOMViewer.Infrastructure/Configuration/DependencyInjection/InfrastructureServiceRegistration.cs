using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheSSS.DICOMViewer.Infrastructure.Persistence.DbContext;

namespace TheSSS.DICOMViewer.Infrastructure.Configuration.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DicomDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IDicomNetworkClient, DicomNetworkClient>();
            services.AddScoped<IDicomImageRenderingService, DicomImageRenderingService>();
            services.AddScoped<IPdfGenerationService, PdfGenerationService>();
            services.AddScoped<ISecureConfigurationHandler, SecureConfigurationHandler>();

            return services;
        }
    }
}