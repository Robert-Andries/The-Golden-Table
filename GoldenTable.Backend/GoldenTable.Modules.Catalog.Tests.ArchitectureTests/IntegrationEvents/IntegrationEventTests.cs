using GoldenTable.Common.Application.EventBus;
using GoldenTable.Modules.Catalog.Tests.ArchitectureTests.Abstractions;
using NetArchTest.Rules;

namespace GoldenTable.Modules.Catalog.Tests.ArchitectureTests.IntegrationEvents;

public class IntegrationEventTests : BaseTest
{
    [Fact]
    public void IntegrationEvents_Should_BeSealed()
    {
        Types.InAssembly(IntegrationEventsAssembly)
            .That()
            .ImplementInterface(typeof(IIntegrationEvent))
            .Or()
            .Inherit(typeof(IntegrationEvent))
            .Should()
            .BeSealed()
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void IntegrationEvent_ShouldHave_IntegrationEventPostfix()
    {
        Types.InAssembly(IntegrationEventsAssembly)
            .That()
            .ImplementInterface(typeof(IIntegrationEvent))
            .Or()
            .Inherit(typeof(IntegrationEvent))
            .Should()
            .HaveNameEndingWith("IntegrationEvent")
            .GetResult()
            .ShouldBeSuccessful();
    }
}
