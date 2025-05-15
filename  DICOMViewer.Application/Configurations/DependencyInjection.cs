using Autofac;
using MediatR;
using System.Reflection;
using TheSSS.DICOMViewer.Application.Common.Behaviours;
using TheSSS.DICOMViewer.Application.Interfaces.Domain;
using TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

namespace TheSSS.DICOMViewer.Application.Configurations;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
            .AsImplementedInterfaces();

        builder.RegisterAssemblyTypes(ThisAssembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>))
            .AsImplementedInterfaces()
            .InstancePerDependency();

        builder.RegisterGeneric(typeof(LoggingBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(ValidationBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(AuditingBehaviour<,>)).As(typeof(IPipelineBehavior<,>));

        builder.RegisterAssemblyTypes(ThisAssembly)
            .Where(t => t.IsClosedTypeOf(typeof(FluentValidation.IValidator<>)))
            .AsImplementedInterfaces()
            .InstancePerDependency();

        builder.RegisterAssemblyTypes(ThisAssembly)
            .Where(t => typeof(AutoMapper.Profile).IsAssignableFrom(t))
            .As<AutoMapper.Profile>();

        builder.Register(c => 
        {
            var config = new AutoMapper.MapperConfiguration(cfg => cfg.AddMaps(ThisAssembly));
            return config.CreateMapper();
        }).As<AutoMapper.IMapper>().SingleInstance();
    }
}