// src/Core/GamingLibrary.Domain/Events/PlayerScoreUpdatedDomainEvent.cs
// Purpose: Domain event fired when player score changes
using GamingLibrary.Domain.Common;
using GamingLibrary.Domain.ValueObjects;

namespace GamingLibrary.Domain.Events
{
    /// <summary>
    /// Domain event raised when a player's score is updated.
    /// Used for leaderboard updates, achievements, notifications.
    /// </summary>
    public class PlayerScoreUpdatedDomainEvent : IDomainEvent
    {
        public PlayerId PlayerId { get; }
        public int OldScore { get; }
        public int NewScore { get; }
        public int ScoreChange { get; }
        public bool GameCompleted { get; }
        public DateTime OccurredOn { get; }

        public PlayerScoreUpdatedDomainEvent(
            PlayerId playerId, 
            int oldScore, 
            int newScore, 
            int scoreChange, 
            bool gameCompleted)
        {
            PlayerId = playerId;
            OldScore = oldScore;
            NewScore = newScore;
            ScoreChange = scoreChange;
            GameCompleted = gameCompleted;
            OccurredOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Indicates if this represents a significant score milestone
        /// </summary>
        public bool IsSignificantMilestone => 
            (NewScore >= 1000 && OldScore < 1000) ||
            (NewScore >= 5000 && OldScore < 5000) ||
            (NewScore >= 10000 && OldScore < 10000);
    }
}