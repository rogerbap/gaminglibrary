// src/Core/GamingLibrary.Domain/Events/PlayerInfoUpdatedDomainEvent.cs
using GamingLibrary.Domain.Common;
using GamingLibrary.Domain.ValueObjects;

namespace GamingLibrary.Domain.Events
{
    /// <summary>
    /// Domain event raised when player information is updated.
    /// </summary>
    public class PlayerInfoUpdatedDomainEvent : IDomainEvent
    {
        public PlayerId PlayerId { get; }
        public PlayerName NewName { get; }
        public Email NewEmail { get; }
        public DateTime OccurredOn { get; }

        public PlayerInfoUpdatedDomainEvent(PlayerId playerId, PlayerName newName, Email newEmail)
        {
            PlayerId = playerId;
            NewName = newName;
            NewEmail = newEmail;
            OccurredOn = DateTime.UtcNow;
        }
    }
}