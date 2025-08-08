// src/Core/GamingLibrary.Domain/Events/PlayerDeactivatedDomainEvent.cs
using GamingLibrary.Domain.Common;
using GamingLibrary.Domain.ValueObjects;

namespace GamingLibrary.Domain.Events
{
    /// <summary>
    /// Domain event raised when a player account is deactivated.
    /// </summary>
    public class PlayerDeactivatedDomainEvent : IDomainEvent
    {
        public PlayerId PlayerId { get; }
        public DateTime OccurredOn { get; }

        public PlayerDeactivatedDomainEvent(PlayerId playerId)
        {
            PlayerId = playerId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
