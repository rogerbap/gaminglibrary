// src/Core/GamingLibrary.Domain/Exceptions/GameSessionDomainException.cs
// Purpose: Game session-specific domain exceptions
namespace GamingLibrary.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when game session business rules are violated
    /// </summary>
    public class GameSessionDomainException : DomainException
    {
        public const string SESSION_NOT_FOUND = "SESSION_NOT_FOUND";
        public const string SESSION_ALREADY_ENDED = "SESSION_ALREADY_ENDED";
        public const string SESSION_TIMEOUT = "SESSION_TIMEOUT";
        public const string INVALID_GAME_TYPE = "INVALID_GAME_TYPE";
        public const string INVALID_SCORE_UPDATE = "INVALID_SCORE_UPDATE";

        public GameSessionDomainException(string message, string errorCode) 
            : base(message, errorCode)
        {
        }

        public GameSessionDomainException(string message, string errorCode, Exception innerException) 
            : base(message, errorCode, innerException)
        {
        }

        public static GameSessionDomainException SessionNotFound(string sessionId) =>
            new($"Game session with ID '{sessionId}' not found", SESSION_NOT_FOUND);

        public static GameSessionDomainException SessionAlreadyEnded(string sessionId) =>
            new($"Game session '{sessionId}' has already ended", SESSION_ALREADY_ENDED);

        public static GameSessionDomainException SessionTimeout(string sessionId, TimeSpan duration) =>
            new($"Game session '{sessionId}' exceeded maximum duration of {GameSession.MaxSessionDuration}. Actual duration: {duration}", 
                SESSION_TIMEOUT);

        public static GameSessionDomainException InvalidScoreUpdate(string sessionId, int attemptedScore) =>
            new($"Invalid score update for session '{sessionId}'. Score cannot be negative: {attemptedScore}", 
                INVALID_SCORE_UPDATE);
    }
}