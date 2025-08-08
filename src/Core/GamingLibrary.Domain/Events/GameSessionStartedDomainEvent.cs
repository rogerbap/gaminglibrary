// src/Core/GamingLibrary.Domain/Events/GameSessionStartedDomainEvent.cs
using GamingLibrary.Domain.Common;
using GamingLibrary.Domain.ValueObjects;
using GamingLibrary.Domain.Enums;

namespace GamingLibrary.Domain.Events
{
    /// <summary>
    /// Domain event raised when a game session starts.
    /// </summary>
    public class GameSessionStartedDomainEvent : IDomainEvent
    {
        public SessionId SessionId { get; }
        public PlayerId PlayerId { get; }
        public GameType GameType { get; }
        public DateTime OccurredOn { get; }

        public GameSessionStartedDomainEvent(SessionId sessionId, PlayerId playerId, GameType gameType)
        {
            SessionId = sessionId;
            PlayerId = playerId;
            GameType = gameType;
            OccurredOn = DateTime.UtcNow;
        }
    }
}