// src/Core/GamingLibrary.Domain/Exceptions/PlayerDomainException.cs
// Purpose: Player-specific domain exceptions
namespace GamingLibrary.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when player business rules are violated
    /// </summary>
    public class PlayerDomainException : DomainException
    {
        public const string INVALID_PLAYER_NAME = "INVALID_PLAYER_NAME";
        public const string INVALID_EMAIL = "INVALID_EMAIL";
        public const string PLAYER_ALREADY_EXISTS = "PLAYER_ALREADY_EXISTS";
        public const string PLAYER_NOT_FOUND = "PLAYER_NOT_FOUND";
        public const string PLAYER_INACTIVE = "PLAYER_INACTIVE";

        public PlayerDomainException(string message, string errorCode) 
            : base(message, errorCode)
        {
        }

        public PlayerDomainException(string message, string errorCode, Exception innerException) 
            : base(message, errorCode, innerException)
        {
        }

        public static PlayerDomainException InvalidPlayerName(string name) =>
            new($"Invalid player name: '{name}'. Must be 2-50 characters and contain no control characters.", 
                INVALID_PLAYER_NAME);

        public static PlayerDomainException InvalidEmail(string email) =>
            new($"Invalid email format: '{email}'", INVALID_EMAIL);

        public static PlayerDomainException PlayerAlreadyExists(string email) =>
            new($"Player with email '{email}' already exists", PLAYER_ALREADY_EXISTS);

        public static PlayerDomainException PlayerNotFound(string playerId) =>
            new($"Player with ID '{playerId}' not found", PLAYER_NOT_FOUND);

        public static PlayerDomainException PlayerInactive(string playerId) =>
            new($"Player '{playerId}' is inactive and cannot perform this action", PLAYER_INACTIVE);
    }
}
