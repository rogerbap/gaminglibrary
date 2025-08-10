// tests/Unit/GamingLibrary.Domain.Tests/Entities/PlayerTests.cs
// Purpose: Unit tests for Player domain entity
using FluentAssertions;
using GamingLibrary.Domain.Entities;
using GamingLibrary.Domain.ValueObjects;
using GamingLibrary.Domain.Events;
using Xunit;

namespace GamingLibrary.Domain.Tests.Entities
{
    /// <summary>
    /// Unit tests for Player domain entity.
    /// Tests business rules, domain events, and encapsulation.
    /// </summary>
    public class PlayerTests
    {
        [Fact]
        public void Create_WithValidData_ShouldCreatePlayerSuccessfully()
        {
            // Arrange
            var name = PlayerName.Create("TestPlayer");
            var email = Email.Create("test@example.com");

            // Act
            var player = Player.Create(name, email);

            // Assert
            player.Should().NotBeNull();
            player.Name.Should().Be(name);
            player.Email.Should().Be(email);
            player.TotalScore.Should().Be(0);
            player.GamesPlayed.Should().Be(0);
            player.IsActive.Should().BeTrue();
            player.PlayerId.Should().NotBeNull();
            
            // Should raise domain event
            player.DomainEvents.Should().ContainSingle(e => e is PlayerCreatedDomainEvent);
        }

        [Fact]
        public void UpdateScore_WithPositiveScore_ShouldUpdateCorrectly()
        {
            // Arrange
            var player = CreateTestPlayer();
            var scoreToAdd = 100;

            // Act
            player.UpdateScore(scoreToAdd, gameCompleted: true);

            // Assert
            player.TotalScore.Should().Be(scoreToAdd);
            player.GamesPlayed.Should().Be(1);
            
            // Should raise domain event
            player.DomainEvents.Should().Contain(e => e is PlayerScoreUpdatedDomainEvent);
        }

        [Fact]
        public void UpdateScore_WithNegativeScoreMakingTotalNegative_ShouldNotGoBelowZero()
        {
            // Arrange
            var player = CreateTestPlayer();
            player.UpdateScore(50, true); // Start with 50 points
            player.ClearDomainEvents(); // Clear initial events

            // Act
            player.UpdateScore(-100, false); // Try to subtract 100

            // Assert
            player.TotalScore.Should().Be(0); // Should not go below 0
            player.GamesPlayed.Should().Be(1); // Should not increment for incomplete game
        }

        [Fact]
        public void QualifiesForLeaderboard_WhenActiveWithGamesPlayed_ShouldReturnTrue()
        {
            // Arrange
            var player = CreateTestPlayer();
            player.UpdateScore(100, true);

            // Act & Assert
            player.QualifiesForLeaderboard().Should().BeTrue();
        }

        [Fact]
        public void QualifiesForLeaderboard_WhenInactive_ShouldReturnFalse()
        {
            // Arrange
            var player = CreateTestPlayer();
            player.UpdateScore(100, true);
            player.Deactivate();

            // Act & Assert
            player.QualifiesForLeaderboard().Should().BeFalse();
        }

        [Fact]
        public void AverageScorePerGame_WithMultipleGames_ShouldCalculateCorrectly()
        {
            // Arrange
            var player = CreateTestPlayer();
            player.UpdateScore(100, true); // Game 1: 100 points
            player.UpdateScore(200, true); // Game 2: 200 points
            player.UpdateScore(300, true); // Game 3: 300 points

            // Act
            var average = player.AverageScorePerGame();

            // Assert
            average.Should().Be(200.0); // (100 + 200 + 300) / 3 = 200
        }

        private static Player CreateTestPlayer()
        {
            var name = PlayerName.Create("TestPlayer");
            var email = Email.Create("test@example.com");
            return Player.Create(name, email);
        }
    }
}
