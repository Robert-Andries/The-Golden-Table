using FluentAssertions;
using NetArchTest.Rules;

namespace GoldenTable.Modules.Catalog.Tests.ArchitectureTests.Abstractions;

internal static class TestResultExtensions
{
    internal static void ShouldBeSuccessful(this TestResult testResult)
    {
        // Compare names, so a failure lists the types that break the rule
        testResult.FailingTypes?.Select(t => t.FullName).Should().BeEmpty();
    }
}
