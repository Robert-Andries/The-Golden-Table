using FluentAssertions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.GlobalRules;

public static class FluentAssertions
{
    public static void ConfigureGlobalSettings()
    {
        AssertionConfiguration.Current.Equivalency.Modify(options => options
                .Excluding(info => info.Path.Contains("DomainEvents", StringComparison.OrdinalIgnoreCase))
                .Excluding(info => info.Path.Contains("ModifiedOnUtc", StringComparison.OrdinalIgnoreCase))
                .Using<DateTime>(ctx =>
                    ctx.Subject.Should()
                        .BeCloseTo(ctx.Expectation,
                            TimeSpan.FromSeconds(1))) // The time will need to be close enough, not precisely that
                .WhenTypeIs<DateTime>()
                .IgnoringCyclicReferences() // Prevent infinite loops if your entities have bidirectional relationships
                .IncludingInternalProperties() // Example 4: Ensure it checks internal properties/fields if you need it to
        );
    }
}
