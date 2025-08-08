// src/Core/GamingLibrary.Domain/Events/PlayerCreatedDomainEvent.cs
// Purpose: Domain event fired when a new player is created
using GamingLibrary.Domain.Common;
using GamingLibrary.Domain.ValueObjects;

namespace GamingLibrary.Domain.Events
{
    /// <summary>
    /// Domain event raised when a new player is created.
    /// Can trigger welcome emails, analytics tracking, etc.
    /// </summary>
    public class PlayerCreatedDomainEvent : IDomainEvent
    {
        public PlayerId PlayerId { get; }
        public PlayerName PlayerName { get; }
        public Email Email { get; }
        public DateTime OccurredOn { get; }

        public PlayerCreatedDomainEvent(PlayerId playerId, PlayerName playerName, Email email)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            Email = email;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
