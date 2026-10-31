using System.Reflection;
using GoldenTable.Common.Application.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;

namespace GoldenTable.Common.Infrastructure.Outbox;

public static class OutboxExtensions
{
    public static IServiceCollection AddOutboxJob<TJob>(
        this IServiceCollection services,
        IConfigurationSection section)
        where TJob : OutboxBaseJob
    {
        string jobName = typeof(TJob).FullName!;
        OutboxOptions outboxOptions = section.Get<OutboxOptions>()
                                      ?? throw new InvalidOperationException($"Missing outbox configuration section '{section.Path}'.");

        if (outboxOptions.IntervalInSeconds <= 0 || outboxOptions.BatchSize <= 0)
        {
            throw new InvalidOperationException($"Invalid outbox configuration in '{section.Path}'.");
        }

        services.Configure<OutboxOptions>(jobName, section);

        services.AddQuartz(q =>
        {
            q.AddJob<TJob>(j => j.WithIdentity(jobName))
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobName)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithInterval(TimeSpan.FromSeconds(outboxOptions.IntervalInSeconds))
                                .RepeatForever()));
        });

        return services;
    }

    public static IServiceCollection AddDomainEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        IEnumerable<Type> handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsAssignableTo(typeof(IDomainEventHandler)));

        foreach (Type handlerType in handlerTypes)
        {
            services.TryAddScoped(handlerType);
        }

        return services;
    }
}
