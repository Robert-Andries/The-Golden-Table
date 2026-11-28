using FluentAssertions;
using GoldenTable.Common.Domain;

namespace GoldenTable.Common.Tests.UnitTests.Results;

public sealed class CreateResult
{
    private static readonly Error SomeError = Error.Failure("Test.Error", "Something went wrong");

    [Fact]
    public void Should_CreateSuccessResult()
    {
        // Act
        var result = Result.Success(5);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
        result.Value.Should().Be(5);
    }

    [Fact]
    public void Should_CreateFailureResult()
    {
        // Act
        var result = Result.Failure<int>(SomeError);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SomeError);
    }

    [Fact]
    public void Should_Throw_WhenSuccessHasAnError()
    {
        // Act
        Action act = () => _ = new Result(true, SomeError);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_Throw_WhenFailureHasNoError()
    {
        // Act
        Action act = () => _ = new Result(false, Error.None);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_Throw_WhenReadingTheValueOfAFailure()
    {
        // Arrange
        var result = Result.Failure<int>(SomeError);

        // Act
        Func<int> act = () => result.Value;

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_CreateNullValueFailure_WhenConvertingNull()
    {
        // Act
        Result<string> result = (string?)null;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }

    [Fact]
    public void Should_CreateFailure_WhenConvertingAnError()
    {
        // Act
        Result result = SomeError;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SomeError);
    }
}
