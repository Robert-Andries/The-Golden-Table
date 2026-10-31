using System.Reflection;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Infrastructure;
using GoldenTable.Modules.Catalog.IntegrationEvents;

namespace GoldenTable.Modules.Catalog.Tests.ArchitectureTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Assembly ApplicationAssembly =
        GoldenTable.Modules.Catalog.Application.AssemblyReference.Assembly;

    protected static readonly Assembly DomainAssembly = typeof(Dish).Assembly;

    protected static readonly Assembly InfrastructureAssembly = typeof(CatalogModule).Assembly;

    protected static readonly Assembly PresentationAssembly =
        GoldenTable.Modules.Catalog.Presentation.AssemblyReference.Assembly;

    protected static readonly Assembly IntegrationEventsAssembly = typeof(DishCreatedIntegrationEvent).Assembly;
}
