using System.Reflection;
using FluentValidation;
using GoldenTable.Common.Application.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace GoldenTable.Common.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services, Assembly[] moduleAssemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(moduleAssemblies);
            config.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));
        });
        services.AddValidatorsFromAssemblies(moduleAssemblies, includeInternalTypes:true);
        return services;
    }
}

