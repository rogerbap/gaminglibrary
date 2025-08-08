// src/Core/GamingLibrary.Domain/Common/IDomainEvent.cs
// Purpose: Marker interface for domain events
namespace GamingLibrary.Domain.Common
{
    /// <summary>
    /// Marker interface for domain events.
    /// Domain events represent important business occurrences.
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// When the domain event occurred
        /// </summary>
        DateTime OccurredOn { get; }
    }
}