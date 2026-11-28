using GoldenTable.Modules.Orders.Tests.ArchitectureTests.Abstractions;
using NetArchTest.Rules;

namespace GoldenTable.Modules.Orders.Tests.ArchitectureTests.Layers;

public class LayerTests : BaseTest
{
    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_ApplicationLayer()
    {
        Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void PresentationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        Types.InAssembly(PresentationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_OtherModules()
    {
        Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny(OtherModulesLayers)
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_OtherModules()
    {
        Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAny(OtherModulesLayers)
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void InfrastructureLayer_ShouldNotHaveDependencyOn_OtherModules()
    {
        Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOnAny(OtherModulesLayers)
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void PresentationLayer_ShouldNotHaveDependencyOn_OtherModules()
    {
        // Other modules may only be reached through their IntegrationEvents projects
        Types.InAssembly(PresentationAssembly)
            .Should()
            .NotHaveDependencyOnAny(OtherModulesLayers)
            .GetResult()
            .ShouldBeSuccessful();
    }
}
