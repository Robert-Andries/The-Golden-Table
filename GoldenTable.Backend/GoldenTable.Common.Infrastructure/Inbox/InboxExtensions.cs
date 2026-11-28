using System.Reflection;
using GoldenTable.Common.Application.EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;

namespace GoldenTable.Common.Infrastructure.Inbox;

public static class InboxExtensions
{
    public static IServiceCollection AddInboxJob<TJob>(
        this IServiceCollection services,
        IConfigurationSection section)
        where TJob : InboxBaseJob
    {
        string jobName = typeof(TJob).FullName!;
        InboxOptions inboxOptions = section.Get<InboxOptions>()
                                    ?? throw new InvalidOperationException(
                                        $"Missing inbox configuration section '{section.Path}'.");

        if (inboxOptions.IntervalInSeconds <= 0 || inboxOptions.BatchSize <= 0)
        {
            throw new InvalidOperationException($"Invalid inbox configuration in '{section.Path}'.");
        }

        services.Configure<InboxOptions>(jobName, section);

        services.AddQuartz(q =>
        {
            q.AddJob<TJob>(j => j.WithIdentity(jobName))
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobName)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithInterval(TimeSpan.FromSeconds(inboxOptions.IntervalInSeconds))
                                .RepeatForever()));
        });

        return services;
    }

    public static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        IEnumerable<Type> handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                        t.IsAssignableTo(typeof(IIntegrationEventHandler)));

        foreach (Type handlerType in handlerTypes)
        {
            services.TryAddScoped(handlerType);
        }

        return services;
    }
}
