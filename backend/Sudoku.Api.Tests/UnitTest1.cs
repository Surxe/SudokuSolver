using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Sudoku.Api.Tests;

public class ExampleIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ExampleIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ExampleTest_ShouldDemonstrateIntegrationTesting()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        // The root endpoint returns 404 since no route is defined
        // This test verifies the API starts and handles requests
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
