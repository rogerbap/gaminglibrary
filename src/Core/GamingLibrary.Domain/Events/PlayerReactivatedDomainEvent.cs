// src/Core/GamingLibrary.Domain/Events/PlayerReactivatedDomainEvent.cs
using GamingLibrary.Domain.Common;
using GamingLibrary.Domain.ValueObjects;

namespace GamingLibrary.Domain.Events
{
    /// <summary>
    /// Domain event raised when a player account is reactivated.
    /// </summary>
    public class PlayerReactivatedDomainEvent : IDomainEvent
    {
        public PlayerId PlayerId { get; }
        public DateTime OccurredOn { get; }

        public PlayerReactivatedDomainEvent(PlayerId playerId)
        {
            PlayerId = playerId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}