// src/Core/GamingLibrary.Domain/Events/GameSessionEndedDomainEvent.cs
using GamingLibrary.Domain.Common;
using GamingLibrary.Domain.ValueObjects;
using GamingLibrary.Domain.Enums;

namespace GamingLibrary.Domain.Events
{
    /// <summary>
    /// Domain event raised when a game session ends.
    /// </summary>
    public class GameSessionEndedDomainEvent : IDomainEvent
    {
        public SessionId SessionId { get; }
        public PlayerId PlayerId { get; }
        public GameType GameType { get; }
        public int FinalScore { get; }
        public bool CompletedSuccessfully { get; }
        public TimeSpan Duration { get; }
        public DateTime OccurredOn { get; }

        public GameSessionEndedDomainEvent(
            SessionId sessionId, 
            PlayerId playerId, 
            GameType gameType,
            int finalScore, 
            bool completedSuccessfully, 
            TimeSpan duration)
        {
            SessionId = sessionId;
            PlayerId = playerId;
            GameType = gameType;
            FinalScore = finalScore;
            CompletedSuccessfully = completedSuccessfully;
            Duration = duration;
            OccurredOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Indicates if this was a high-performance session
        /// </summary>
        public bool IsHighPerformance => CompletedSuccessfully && FinalScore >= 1000;
    }
}