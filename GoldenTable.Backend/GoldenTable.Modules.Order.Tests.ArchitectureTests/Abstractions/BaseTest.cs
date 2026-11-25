using System.Reflection;
using GoldenTable.Modules.Order.Domain.Dishes;

namespace GoldenTable.Modules.Order.Tests.ArchitectureTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Assembly ApplicationAssembly =
        GoldenTable.Modules.Order.Application.AssemblyReference.Assembly;

    protected static readonly Assembly DomainAssembly = typeof(Dish).Assembly;

    protected static readonly Assembly InfrastructureAssembly =
        GoldenTable.Modules.Order.Infrastructure.AssemblyReference.Assembly;

    protected static readonly Assembly PresentationAssembly =
        GoldenTable.Modules.Order.Presentation.AssemblyReference.Assembly;

    /// <summary>
    ///     Layers of other modules the Order module must never reference directly.
    ///     Only their IntegrationEvents projects may be used for cross-module communication.
    /// </summary>
    protected static readonly string[] OtherModulesLayers =
    [
        "GoldenTable.Modules.Catalog.Domain",
        "GoldenTable.Modules.Catalog.Application",
        "GoldenTable.Modules.Catalog.Infrastructure",
        "GoldenTable.Modules.Catalog.Presentation"
    ];
}
