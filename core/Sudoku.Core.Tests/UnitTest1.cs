using FluentAssertions;
using Xunit;

namespace Sudoku.Core.Tests;

public class ExampleTests
{
    [Fact]
    public void ExampleTest_ShouldDemonstrateFluentAssertions()
    {
        // Arrange
        var expected = 5;
        var actual = 2 + 3;

        // Act & Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(5, 10, 15)]
    [InlineData(-1, 1, 0)]
    public void ExampleTheoryTest_ShouldWorkWithMultipleInputs(int a, int b, int expected)
    {
        // Act
        var result = a + b;

        // Assert
        result.Should().Be(expected);
    }
}
