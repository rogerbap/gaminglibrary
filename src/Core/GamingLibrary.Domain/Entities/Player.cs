// src/Core/GamingLibrary.Domain/Entities/Player.cs
// Purpose: Core Player domain entity with business logic
using GamingLibrary.Domain.Common;
using GamingLibrary.Domain.ValueObjects;
using GamingLibrary.Domain.Events;

namespace GamingLibrary.Domain.Entities
{
    /// <summary>
    /// Player aggregate root representing a user in the gaming library.
    /// Contains player statistics and business rules for scoring.
    /// </summary>
    public class Player : BaseEntity
    {
        // Private fields for encapsulation
        private int _totalScore;
        private int _gamesPlayed;

        /// <summary>
        /// Unique player identifier
        /// </summary>
        public PlayerId PlayerId { get; private set; }

        /// <summary>
        /// Player's display name
        /// </summary>
        public PlayerName Name { get; private set; }

        /// <summary>
        /// Player's email address
        /// </summary>
        public Email Email { get; private set; }

        /// <summary>
        /// Total score across all games (read-only public access)
        /// </summary>
        public int TotalScore => _totalScore;

        /// <summary>
        /// Total number of games played (read-only public access)
        /// </summary>
        public int GamesPlayed => _gamesPlayed;

        /// <summary>
        /// When the player last played a game
        /// </summary>
        public DateTime LastPlayedAt { get; private set; }

        /// <summary>
        /// Whether the player account is active
        /// </summary>
        public bool IsActive { get; private set; } = true;

        // Private constructor for EF Core / MongoDB
        private Player() { }

        /// <summary>
        /// Creates a new player with domain validation
        /// </summary>
        public static Player Create(PlayerName name, Email email)
        {
            var player = new Player
            {
                PlayerId = PlayerId.CreateNew(),
                Name = name,
                Email = email,
                _totalScore = 0,
                _gamesPlayed = 0,
                LastPlayedAt = DateTime.UtcNow
            };

            // Raise domain event
            player.AddDomainEvent(new PlayerCreatedDomainEvent(player.PlayerId, player.Name, player.Email));

            return player;
        }

        /// <summary>
        /// Updates the player's score after completing a game
        /// Includes business rules for scoring validation
        /// </summary>
        /// <param name="additionalScore">Score to add (can be negative for penalties)</param>
        /// <param name="gameCompleted">Whether the game was completed successfully</param>
        public void UpdateScore(int additionalScore, bool gameCompleted)
        {
            // Business rule: No negative total scores
            var newScore = Math.Max(0, _totalScore + additionalScore);
            
            var oldScore = _totalScore;
            _totalScore = newScore;
            
            if (gameCompleted)
            {
                _gamesPlayed++;
            }
            
            LastPlayedAt = DateTime.UtcNow;
            MarkAsModified();

            // Raise domain event for score changes
            AddDomainEvent(new PlayerScoreUpdatedDomainEvent(
                PlayerId, oldScore, newScore, additionalScore, gameCompleted));
        }

        /// <summary>
        /// Updates player information
        /// </summary>
        public void UpdateInfo(PlayerName name, Email email)
        {
            Name = name;
            Email = email;
            MarkAsModified();

            AddDomainEvent(new PlayerInfoUpdatedDomainEvent(PlayerId, name, email));
        }

        /// <summary>
        /// Deactivates the player account
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            MarkAsModified();

            AddDomainEvent(new PlayerDeactivatedDomainEvent(PlayerId));
        }

        /// <summary>
        /// Reactivates the player account
        /// </summary>
        public void Reactivate()
        {
            IsActive = true;
            LastPlayedAt = DateTime.UtcNow;
            MarkAsModified();

            AddDomainEvent(new PlayerReactivatedDomainEvent(PlayerId));
        }

        /// <summary>
        /// Records that the player started a game session
        /// </summary>
        public void RecordGameStart()
        {
            LastPlayedAt = DateTime.UtcNow;
            MarkAsModified();
        }

        /// <summary>
        /// Business rule: Check if player qualifies for leaderboard
        /// </summary>
        public bool QualifiesForLeaderboard() => IsActive && GamesPlayed >= 1;

        /// <summary>
        /// Business rule: Calculate player's average score per game
        /// </summary>
        public double AverageScorePerGame() => GamesPlayed > 0 ? (double)TotalScore / GamesPlayed : 0;
    }
}