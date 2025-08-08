// src/Core/GamingLibrary.Domain/Common/BaseEntity.cs
// Purpose: Base class for all domain entities with common functionality
using System.ComponentModel.DataAnnotations;

namespace GamingLibrary.Domain.Common
{
    /// <summary>
    /// Base entity class providing common functionality for all domain entities.
    /// Follows Domain-Driven Design principles with proper encapsulation.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Unique identifier for the entity (MongoDB ObjectId as string)
        /// </summary>
        public string Id { get; protected set; } = string.Empty;

        /// <summary>
        /// When the entity was created
        /// </summary>
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

        /// <summary>
        /// When the entity was last modified
        /// </summary>
        public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

        /// <summary>
        /// Updates the UpdatedAt timestamp
        /// Should be called by domain methods that modify state
        /// </summary>
        protected void MarkAsModified()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Domain events that occurred during entity operations
        /// Used for triggering side effects in a clean way
        /// </summary>
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Adds a domain event to be processed later
        /// </summary>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Clears all domain events (called after processing)
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
