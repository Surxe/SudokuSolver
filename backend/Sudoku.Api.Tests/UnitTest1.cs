using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Sudoku.Api.DTOs;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Sudoku.Api.Tests;

public class SudokuApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SudokuApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetRandomPuzzle_ShouldReturnPuzzle()
    {
        // Act
        var response = await _client.GetAsync("/api/sudoku/puzzle");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        
        var puzzle = await response.Content.ReadFromJsonAsync<PuzzleResponse>();
        puzzle.Should().NotBeNull();
        puzzle!.Id.Should().BeGreaterThan(0);
        puzzle.Board.Should().NotBeNull();
        puzzle.Board.Should().HaveCount(9);
        puzzle.Board[0].Should().HaveCount(9);
    }

    [Fact]
    public async Task GetPuzzleById_WithValidId_ShouldReturnPuzzle()
    {
        // Act
        var response = await _client.GetAsync("/api/sudoku/puzzle/1");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        
        var puzzle = await response.Content.ReadFromJsonAsync<PuzzleResponse>();
        puzzle.Should().NotBeNull();
        puzzle!.Id.Should().Be(1);
        puzzle.Board.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPuzzleById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/sudoku/puzzle/99999");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SolvePuzzle_WithValidBoard_ShouldReturnSolution()
    {
        // Arrange
        var request = new SolveRequest
        {
            Board = new int[][]
            {
                new int[] {5, 3, 0, 0, 7, 0, 0, 0, 0},
                new int[] {6, 0, 0, 1, 9, 5, 0, 0, 0},
                new int[] {0, 9, 8, 0, 0, 0, 0, 6, 0},
                new int[] {8, 0, 0, 0, 6, 0, 0, 0, 3},
                new int[] {4, 0, 0, 8, 0, 3, 0, 0, 1},
                new int[] {7, 0, 0, 0, 2, 0, 0, 0, 6},
                new int[] {0, 6, 0, 0, 0, 0, 2, 8, 0},
                new int[] {0, 0, 0, 4, 1, 9, 0, 0, 5},
                new int[] {0, 0, 0, 0, 8, 0, 0, 7, 9}
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sudoku/solve", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        
        var solveResponse = await response.Content.ReadFromJsonAsync<SolveResponse>();
        solveResponse.Should().NotBeNull();
        solveResponse!.Solved.Should().BeTrue();
        solveResponse.Solution.Should().NotBeNull();
        solveResponse.Solution.Should().HaveCount(9);
        solveResponse.Attempts.Should().BeGreaterThan(0);
        solveResponse.DurationMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SolvePuzzle_WithInvalidBoard_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new SolveRequest
        {
            Board = new int[][]
            {
                new int[] {5, 3, 5, 0, 7, 0, 0, 0, 0}, // Duplicate 5 in row
                new int[] {6, 0, 0, 1, 9, 5, 0, 0, 0},
                new int[] {0, 9, 8, 0, 0, 0, 0, 6, 0},
                new int[] {8, 0, 0, 0, 6, 0, 0, 0, 3},
                new int[] {4, 0, 0, 8, 0, 3, 0, 0, 1},
                new int[] {7, 0, 0, 0, 2, 0, 0, 0, 6},
                new int[] {0, 6, 0, 0, 0, 0, 2, 8, 0},
                new int[] {0, 0, 0, 4, 1, 9, 0, 0, 5},
                new int[] {0, 0, 0, 0, 8, 0, 0, 7, 9}
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sudoku/solve", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        
        var solveResponse = await response.Content.ReadFromJsonAsync<SolveResponse>();
        solveResponse.Should().NotBeNull();
        solveResponse!.Solved.Should().BeFalse();
        solveResponse.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SolvePuzzle_WithMalformedBoard_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new SolveRequest
        {
            Board = new int[][]
            {
                new int[] {5, 3, 0}, // Invalid size
                new int[] {6, 0, 0}
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sudoku/solve", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
