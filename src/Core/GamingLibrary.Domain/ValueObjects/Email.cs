// src/Core/GamingLibrary.Domain/ValueObjects/Email.cs
// Purpose: Email value object with validation
using GamingLibrary.Domain.Common;
using System.Text.RegularExpressions;

namespace GamingLibrary.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing an email address with validation.
    /// </summary>
    public class Email : ValueObject
    {
        private static readonly Regex EmailRegex = new(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        private Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be null or empty", nameof(value));

            if (!EmailRegex.IsMatch(value))
                throw new ArgumentException("Invalid email format", nameof(value));

            Value = value.ToLowerInvariant().Trim();
        }

        public static Email Create(string value) => new(value);

        public static implicit operator string(Email email) => email.Value;
        public static explicit operator Email(string value) => Create(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}