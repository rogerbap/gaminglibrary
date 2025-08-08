// src/Core/GamingLibrary.Domain/ValueObjects/PlayerName.cs
// Purpose: Player name value object with validation
using GamingLibrary.Domain.Common;

namespace GamingLibrary.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing a player's display name with validation.
    /// </summary>
    public class PlayerName : ValueObject
    {
        public const int MinLength = 2;
        public const int MaxLength = 50;

        public string Value { get; }

        private PlayerName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Player name cannot be null or empty", nameof(value));

            var trimmed = value.Trim();
            
            if (trimmed.Length < MinLength)
                throw new ArgumentException($"Player name must be at least {MinLength} characters", nameof(value));

            if (trimmed.Length > MaxLength)
                throw new ArgumentException($"Player name cannot exceed {MaxLength} characters", nameof(value));

            // Basic sanitization - remove any potentially harmful characters
            if (trimmed.Any(c => char.IsControl(c)))
                throw new ArgumentException("Player name cannot contain control characters", nameof(value));

            Value = trimmed;
        }

        public static PlayerName Create(string value) => new(value);

        public static implicit operator string(PlayerName playerName) => playerName.Value;
        public static explicit operator PlayerName(string value) => Create(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
