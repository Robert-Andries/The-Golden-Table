using FluentAssertions;
using GoldenTable.Common.Domain;

namespace GoldenTable.Common.Tests.UnitTests.Results;

public sealed class CreateValidationError
{
    [Fact]
    public void Should_KeepOnlyTheErrorsOfFailedResults()
    {
        // Arrange
        var first = Error.Failure("Test.First", "First error");
        var second = Error.Failure("Test.Second", "Second error");
        Result[] results = [Result.Success(), Result.Failure(first), Result.Success(), Result.Failure(second)];

        // Act
        var validationError = ValidationError.FromResults(results);

        // Assert
        validationError.Type.Should().Be(ErrorType.Validation);
        validationError.Errors.Should().Equal(first, second);
    }
}
