using System.Net;
using System.Net.Http.Json;
using Backend.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Backend.Tests.Integration
{
    public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SignUp_WithValidData_ReturnsToken()
        {
            // Arrange
            var credentials = new UserCredentialsData
            {
                UserName = $"test_{Guid.NewGuid().ToString().Substring(0, 6)}",
                Password = "password123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/signup", credentials);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var credentials = new UserCredentialsData
            {
                UserName = $"loginuser_123",
                Password = "password123"
            };

            // First sign up
            await _client.PostAsJsonAsync("/api/auth/signup", credentials);

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", credentials);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var credentials = new UserCredentialsData
            {
                UserName = "nonexistent",
                Password = "wrongpassword"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", credentials);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task SignUp_WithExistingUser_ReturnsBadRequest()
        {
            // Arrange
            var credentials = new UserCredentialsData
            {
                UserName = $"duplicate_{Guid.NewGuid()}",
                Password = "password123"
            };

            // First sign up
            await _client.PostAsJsonAsync("/api/auth/signup", credentials);

            // Act - Try to sign up again with same username
            var response = await _client.PostAsJsonAsync("/api/auth/signup", credentials);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private class TokenResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
