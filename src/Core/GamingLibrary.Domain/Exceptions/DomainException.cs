// src/Core/GamingLibrary.Domain/Exceptions/DomainException.cs
// Purpose: Base exception for all domain-related errors
namespace GamingLibrary.Domain.Exceptions
{
    /// <summary>
    /// Base exception for domain-related errors.
    /// Represents violations of business rules or domain invariants.
    /// </summary>
    public abstract class DomainException : Exception
    {
        /// <summary>
        /// Error code for programmatic handling
        /// </summary>
        public string ErrorCode { get; }

        protected DomainException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        protected DomainException(string message, string errorCode, Exception innerException) 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}