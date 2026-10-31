using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GoldenTable.Common.Tests.UnitTests.Outbox;

public sealed class ProcessOutboxMessages : IDisposable
{
    private const int BatchSize = 7;

    // Same settings as the interceptor that writes the outbox messages
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };

    private readonly HandlerCalls _handlerCalls = new();
    private readonly ServiceProvider _serviceProvider;
    private readonly TestOutboxJob _sut;

    public ProcessOutboxMessages()
    {
        ServiceCollection services = new();
        services.AddSingleton(_handlerCalls);
        services.AddDomainEventHandlers(typeof(ProcessOutboxMessages).Assembly);
        services.Configure<OutboxOptions>(typeof(TestOutboxJob).FullName, options => options.BatchSize = BatchSize);
        _serviceProvider = services.BuildServiceProvider();

        _sut = new TestOutboxJob(
            _serviceProvider.GetRequiredService<IOptionsMonitor<OutboxOptions>>(),
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
        SingleHandlerDomainEvent domainEvent = new();
        _sut.AddMessage(domainEvent.Id, Serialize(domainEvent));

        // Act
        await _sut.Execute(null!, CancellationToken.None);

        // Assert
        _handlerCalls.Calls.Should().ContainSingle()
            .Which.Should().Be((nameof(RecordingDomainEventHandler), domainEvent.Id));
        _sut.Consumers.Should().Contain((domainEvent.Id, typeof(RecordingDomainEventHandler).FullName!));
        _sut.ProcessedMessages.Should().ContainKey(domainEvent.Id).WhoseValue.Should().BeNull();
    }

    [Fact]
    public async Task Should_SkipHandler_WhenItAlreadyConsumedTheMessage()
    {
        // Arrange
        SingleHandlerDomainEvent domainEvent = new();
        _sut.AddMessage(domainEvent.Id, Serialize(domainEvent));
        _sut.Consumers.Add((domainEvent.Id, typeof(RecordingDomainEventHandler).FullName!));

        // Act
        await _sut.Execute(null!, CancellationToken.None);

        // Assert
        _handlerCalls.Calls.Should().BeEmpty();
        _sut.ProcessedMessages.Should().ContainKey(domainEvent.Id).WhoseValue.Should().BeNull();
    }

    [Fact]
    public async Task Should_RunTheOtherHandlers_WhenOneHandlerFails()
    {
        // Arrange
        PartiallyFailingDomainEvent domainEvent = new();
        _sut.AddMessage(domainEvent.Id, Serialize(domainEvent));

        // Act
        await _sut.Execute(null!, CancellationToken.None);

        // Assert
        _handlerCalls.Calls.Should().ContainSingle()
            .Which.Should().Be((nameof(SucceedingDomainEventHandler), domainEvent.Id));
        _sut.Consumers.Should().Contain((domainEvent.Id, typeof(SucceedingDomainEventHandler).FullName!));
        _sut.Consumers.Should().NotContain((domainEvent.Id, typeof(FailingDomainEventHandler).FullName!));
        _sut.ProcessedMessages.Should().ContainKey(domainEvent.Id)
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
    public async Task Should_KeepEveryDecimalDigit_WhenDeserializing()
    {
        // Arrange
        AmountDomainEvent domainEvent = new(768.466413572458510m);
        _sut.AddMessage(domainEvent.Id, Serialize(domainEvent));

        // Act
        await _sut.Execute(null!, CancellationToken.None);

        // Assert
        _handlerCalls.Amounts.Should().ContainSingle().Which.Should().Be(768.466413572458510m);
    }

    [Fact]
    public async Task Should_ReadTheConfiguredBatchSize()
    {
        // Act
        await _sut.Execute(null!, CancellationToken.None);

        // Assert
        _sut.RequestedBatchSize.Should().Be(BatchSize);
    }

    private static string Serialize(IDomainEvent domainEvent)
    {
        return JsonConvert.SerializeObject(domainEvent, SerializerSettings);
    }
}
