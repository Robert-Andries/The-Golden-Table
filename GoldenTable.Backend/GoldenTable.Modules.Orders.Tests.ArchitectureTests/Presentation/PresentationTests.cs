using FluentAssertions;
using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Modules.Orders.Tests.ArchitectureTests.Abstractions;
using MassTransit;
using NetArchTest.Rules;

namespace GoldenTable.Modules.Orders.Tests.ArchitectureTests.Presentation;

public class PresentationTests : BaseTest
{
    [Fact]
    public void Endpoint_Should_BeSealed()
    {
        // AddEndpoints skips abstract classes, so a sealed endpoint is also one that gets mapped
        Types.InAssembly(PresentationAssembly)
            .That()
            .ImplementInterface(typeof(IEndpoint))
            .Should()
            .BeSealed()
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Endpoint_Should_NotBePublic()
    {
        Types.InAssembly(PresentationAssembly)
            .That()
            .ImplementInterface(typeof(IEndpoint))
            .Should()
            .NotBePublic()
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void Presentation_ShouldNotHave_MassTransitConsumers()
    {
        // Integration events must go through the inbox: the generic IntegrationEventConsumer stores them,
        // and integration event handlers process them
        IEnumerable<Type> consumers = Types.InAssembly(PresentationAssembly)
            .That()
            .ImplementInterface(typeof(IConsumer<>))
            .GetTypes();

        consumers.Select(t => t.FullName).Should().BeEmpty();
    }

    [Fact]
    public void IntegrationEventHandler_Should_BeSealed()
    {
        Types.InAssembly(PresentationAssembly)
            .That()
            .ImplementInterface(typeof(IIntegrationEventHandler<>))
            .Or()
            .Inherit(typeof(IntegrationEventHandler<>))
            .Should()
            .BeSealed()
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void IntegrationEventHandler_Should_NotBePublic()
    {
        Types.InAssembly(PresentationAssembly)
            .That()
            .ImplementInterface(typeof(IIntegrationEventHandler<>))
            .Or()
            .Inherit(typeof(IntegrationEventHandler<>))
            .Should()
            .NotBePublic()
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void IntegrationEventHandler_ShouldHave_NameEndingWith_IntegrationEventHandler()
    {
        Types.InAssembly(PresentationAssembly)
            .That()
            .ImplementInterface(typeof(IIntegrationEventHandler<>))
            .Or()
            .Inherit(typeof(IntegrationEventHandler<>))
            .Should()
            .HaveNameEndingWith("IntegrationEventHandler")
            .GetResult()
            .ShouldBeSuccessful();
    }
}
