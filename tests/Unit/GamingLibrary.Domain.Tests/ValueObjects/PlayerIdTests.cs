// tests/Unit/GamingLibrary.Domain.Tests/ValueObjects/PlayerIdTests.cs
// Purpose: Unit tests for PlayerId value object
using FluentAssertions;
using GamingLibrary.Domain.ValueObjects;
using Xunit;

namespace GamingLibrary.Domain.Tests.ValueObjects
{
    /// <summary>
    /// Unit tests for PlayerId value object.
    /// Tests validation, equality, and immutability.
    /// </summary>
    public class PlayerIdTests
    {
        [Fact]
        public void Create_WithValidGuid_ShouldCreateSuccessfully()
        {
            // Arrange
            var validGuid = Guid.NewGuid().ToString();

            // Act
            var playerId = PlayerId.Create(validGuid);

            // Assert
            playerId.Should().NotBeNull();
            playerId.Value.Should().Be(validGuid);
        }

        [Fact]
        public void Create_WithInvalidGuid_ShouldThrowArgumentException()
        {
            // Arrange
            var invalidGuid = "not-a-guid";

            // Act & Assert
            var act = () => PlayerId.Create(invalidGuid);
            act.Should().Throw<ArgumentException>()
                .WithMessage("PlayerId must be a valid GUID*");
        }

        [Fact]
        public void Create_WithNullOrEmpty_ShouldThrowArgumentException()
        {
            // Act & Assert
            var act1 = () => PlayerId.Create(string.Empty);
            act1.Should().Throw<ArgumentException>()
                .WithMessage("PlayerId cannot be null or empty*");

            var act2 = () => PlayerId.Create(null!);
            act2.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CreateNew_ShouldGenerateValidGuid()
        {
            // Act
            var playerId = PlayerId.CreateNew();

            // Assert
            playerId.Should().NotBeNull();
            Guid.TryParse(playerId.Value, out _).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithSameValue_ShouldBeEqual()
        {
            // Arrange
            var guid = Guid.NewGuid().ToString();
            var playerId1 = PlayerId.Create(guid);
            var playerId2 = PlayerId.Create(guid);

            // Act & Assert
            playerId1.Should().Be(playerId2);
            playerId1.GetHashCode().Should().Be(playerId2.GetHashCode());
        }

        [Fact]
        public void Equals_WithDifferentValue_ShouldNotBeEqual()
        {
            // Arrange
            var playerId1 = PlayerId.CreateNew();
            var playerId2 = PlayerId.CreateNew();

            // Act & Assert
            playerId1.Should().NotBe(playerId2);
        }

        [Fact]
        public void ImplicitConversion_ToStringVVV_ShouldWork()
        {
            // Arrange
            var guid = Guid.NewGuid().ToString();
            var playerId = PlayerId.Create(guid);

            // Act
            string stringValue = playerId;

            // Assert
            stringValue.Should().Be(guid);
        }
    }
}
