// tests/Integration/GamingLibrary.Api.IntegrationTests/Controllers/PlayersControllerTests.cs
// Purpose: Integration tests for Players API controller
using FluentAssertions;
using GamingLibrary.Api.Controllers;
using GamingLibrary.Application.Features.Players.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace GamingLibrary.Api.IntegrationTests.Controllers
{
    /// <summary>
    /// Integration tests for PlayersController.
    /// Tests the full HTTP request/response cycle with in-memory database.
    /// </summary>
    public class PlayersControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public PlayersControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreatePlayer_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var request = new CreatePlayerRequest
            {
                Name = "Integration Test Player",
                Email = $"test-{Guid.NewGuid()}@example.com"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/players", request);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var createdPlayer = await response.Content.ReadFromJsonAsync<PlayerResponse>();
            createdPlayer.Should().NotBeNull();
            createdPlayer!.Name.Should().Be(request.Name);
            createdPlayer.Email.Should().Be(request.Email);
            createdPlayer.PlayerId.Should().NotBeNullOrEmpty();
            createdPlayer.TotalScore.Should().Be(0);
        }

        [Fact]
        public async Task CreatePlayer_WithInvalidEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreatePlayerRequest
            {
                Name = "Test Player",
                Email = "invalid-email"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/players", request);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetPlayerById_WithExistingPlayer_ShouldReturnPlayer()
        {
            // Arrange - First create a player
            var createRequest = new CreatePlayerRequest
            {
                Name = "Get Test Player",
                Email = $"gettest-{Guid.NewGuid()}@example.com"
            };
            
            var createResponse = await _client.PostAsJsonAsync("/api/v1/players", createRequest);
            var createdPlayer = await createResponse.Content.ReadFromJsonAsync<PlayerResponse>();

            // Act
            var getResponse = await _client.GetAsync($"/api/v1/players/{createdPlayer!.PlayerId}");
            
            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var retrievedPlayer = await getResponse.Content.ReadFromJsonAsync<PlayerResponse>();
            retrievedPlayer.Should().NotBeNull();
            retrievedPlayer!.PlayerId.Should().Be(createdPlayer.PlayerId);
            retrievedPlayer.Name.Should().Be(createdPlayer.Name);
        }

        [Fact]
        public async Task GetPlayerById_WithNonExistentPlayer_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid().ToString();

            // Act
            var response = await _client.GetAsync($"/api/v1/players/{nonExistentId}");
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}