using FluentAssertions;
using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Infrastructure.Inbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GoldenTable.Common.Tests.UnitTests.Inbox;

public sealed class ProcessInboxMessages : IDisposable
{
    private const int BatchSize = 7;

    // Same settings as the consumer that writes the inbox messages
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };

    private readonly HandlerCalls _handlerCalls = new();
    private readonly ServiceProvider _serviceProvider;
    private readonly TestInboxJob _sut;

    public ProcessInboxMessages()
    {
        ServiceCollection services = new();
        services.AddSingleton(_handlerCalls);
        services.AddIntegrationEventHandlers(typeof(ProcessInboxMessages).Assembly);
        services.Configure<InboxOptions>(typeof(TestInboxJob).FullName, options => options.BatchSize = BatchSize);
        _serviceProvider = services.BuildServiceProvider();

        _sut = new TestInboxJob(
            _serviceProvider.GetRequiredService<IOptionsMonitor<InboxOptions>>(),
            _serviceProvider.GetRequiredService<IServiceScopeFactory>());
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }

    [Fact]
    public async Task Should_RunHandler_AndMarkItAsConsumed()
    {
        // Arrange
        SingleHandlerIntegrationEvent integrationEvent = new(Guid.NewGuid(), DateTime.UtcNow);
        _sut.AddMessage(integrationEvent.Id, Serialize(integrationEvent));

        // Act
        await _sut.Execute(null!, CancellationToken.None);

        // Assert
        _handlerCalls.Calls.Should().ContainSingle()
            .Which.Should().Be((nameof(RecordingIntegrationEventHandler), integrationEvent.Id));
        _sut.Consumers.Should().Contain((integrationEvent.Id, typeof(RecordingIntegrationEventHandler).FullName!));
        _sut.ProcessedMessages.Should().ContainKey(integrationEvent.Id).WhoseValue.Should().BeNull();
    }

    [Fact]
    public async Task Should_SkipHandler_WhenItAlreadyConsumedTheMessage()
    {
        // Arrange
        SingleHandlerIntegrationEvent integrationEvent = new(Guid.NewGuid(), DateTime.UtcNow);
        _sut.AddMessage(integrationEvent.Id, Serialize(integrationEvent));
        _sut.Consumers.Add((integrationEvent.Id, typeof(RecordingIntegrationEventHandler).FullName!));

        // Act
        await _sut.Execute(null!, CancellationToken.None);

        // Assert
        _handlerCalls.Calls.Should().BeEmpty();
        _sut.ProcessedMessages.Should().ContainKey(integrationEvent.Id).WhoseValue.Should().BeNull();
    }

    [Fact]
    public async Task Should_RunTheOtherHandlers_WhenOneHandlerFails()
    {
        // Arrange
        PartiallyFailingIntegrationEvent integrationEvent = new(Guid.NewGuid(), DateTime.UtcNow);
        _sut.AddMessage(integrationEvent.Id, Serialize(integrationEvent));

        // Act
        await _sut.Execute(null!, CancellationToken.None);

        // Assert
        _handlerCalls.Calls.Should().ContainSingle()
            .Which.Should().Be((nameof(SucceedingIntegrationEventHandler), integrationEvent.Id));
        _sut.Consumers.Should().Contain((integrationEvent.Id, typeof(SucceedingIntegrationEventHandler).FullName!));
        _sut.Consumers.Should().NotContain((integrationEvent.Id, typeof(FailingIntegrationEventHandler).FullName!));
        _sut.ProcessedMessages.Should().ContainKey(integrationEvent.Id)
            .WhoseValue.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task Should_RecordError_WhenTheContentCannotBeDeserialized()
    {
        // Arrange
        var messageId = Guid.NewGuid();
        _sut.AddMessage(messageId, "not json");

        // Act
        await _sut.Execute(null!, CancellationToken.None);

        // Assert
        _handlerCalls.Calls.Should().BeEmpty();
        _sut.ProcessedMessages.Should().ContainKey(messageId).WhoseValue.Should().BeAssignableTo<JsonException>();
    }

    [Fact]
    public async Task Should_ReadTheConfiguredBatchSize()
    {
        // Act
        await _sut.Execute(null!, CancellationToken.None);

        // Assert
        _sut.RequestedBatchSize.Should().Be(BatchSize);
    }

    private static string Serialize(IIntegrationEvent integrationEvent)
    {
        return JsonConvert.SerializeObject(integrationEvent, SerializerSettings);
    }
}
