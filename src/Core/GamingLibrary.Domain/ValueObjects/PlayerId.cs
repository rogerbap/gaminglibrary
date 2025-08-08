// src/Core/GamingLibrary.Domain/ValueObjects/PlayerId.cs
// Purpose: Strongly-typed identifier for players
using GamingLibrary.Domain.Common;

namespace GamingLibrary.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing a unique player identifier.
    /// Provides type safety and validation for player IDs.
    /// </summary>
    public class PlayerId : ValueObject
    {
        public string Value { get; }

        private PlayerId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("PlayerId cannot be null or empty", nameof(value));
            
            if (!Guid.TryParse(value, out _))
                throw new ArgumentException("PlayerId must be a valid GUID", nameof(value));

            Value = value;
        }

        /// <summary>
        /// Creates a new PlayerId from a string value
        /// </summary>
        public static PlayerId Create(string value) => new(value);

        /// <summary>
        /// Creates a new random PlayerId
        /// </summary>
        public static PlayerId CreateNew() => new(Guid.NewGuid().ToString());

        /// <summary>
        /// Implicit conversion to string for ease of use
        /// </summary>
        public static implicit operator string(PlayerId playerId) => playerId.Value;

        /// <summary>
        /// Explicit conversion from string
        /// </summary>
        public static explicit operator PlayerId(string value) => Create(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
