// src/Core/GamingLibrary.Domain/ValueObjects/SessionId.cs
// Purpose: Strongly-typed identifier for game sessions
using GamingLibrary.Domain.Common;

namespace GamingLibrary.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing a unique game session identifier.
    /// </summary>
    public class SessionId : ValueObject
    {
        public string Value { get; }

        private SessionId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SessionId cannot be null or empty", nameof(value));
            
            if (!Guid.TryParse(value, out _))
                throw new ArgumentException("SessionId must be a valid GUID", nameof(value));

            Value = value;
        }

        public static SessionId Create(string value) => new(value);
        public static SessionId CreateNew() => new(Guid.NewGuid().ToString());

        public static implicit operator string(SessionId sessionId) => sessionId.Value;
        public static explicit operator SessionId(string value) => Create(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}