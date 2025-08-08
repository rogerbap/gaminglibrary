// src/Core/GamingLibrary.Domain/Exceptions/ValidationException.cs
// Purpose: Value object and input validation exceptions  
namespace GamingLibrary.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when input validation fails
    /// </summary>
    public class ValidationException : DomainException
    {
        public const string REQUIRED_FIELD_MISSING = "REQUIRED_FIELD_MISSING";
        public const string INVALID_FORMAT = "INVALID_FORMAT";
        public const string VALUE_OUT_OF_RANGE = "VALUE_OUT_OF_RANGE";

        public string? FieldName { get; }

        public ValidationException(string message, string errorCode, string? fieldName = null) 
            : base(message, errorCode)
        {
            FieldName = fieldName;
        }

        public static ValidationException RequiredFieldMissing(string fieldName) =>
            new($"Required field '{fieldName}' is missing or empty", REQUIRED_FIELD_MISSING, fieldName);

        public static ValidationException InvalidFormat(string fieldName, string value, string expectedFormat) =>
            new($"Field '{fieldName}' has invalid format. Value: '{value}', Expected: {expectedFormat}", 
                INVALID_FORMAT, fieldName);

        public static ValidationException ValueOutOfRange(string fieldName, object value, object min, object max) =>
            new($"Field '{fieldName}' value '{value}' is out of range. Must be between {min} and {max}", 
                VALUE_OUT_OF_RANGE, fieldName);
    }
}