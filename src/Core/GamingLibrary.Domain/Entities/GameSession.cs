// src/Core/GamingLibrary.Domain/Entities/GameSession.cs
// Purpose: Game session aggregate root with business logic
using GamingLibrary.Domain.Common;
using GamingLibrary.Domain.ValueObjects;
using GamingLibrary.Domain.Enums;
using GamingLibrary.Domain.Events;

namespace GamingLibrary.Domain.Entities
{
    /// <summary>
    /// GameSession aggregate root representing a single game playthrough.
    /// Contains session state, scoring logic, and timing information.
    /// </summary>
    public class GameSession : BaseEntity
    {
        // Private fields for encapsulation
        private int _score;
        private DateTime? _endTime;
        private readonly Dictionary<string, object> _gameSpecificData;

        /// <summary>
        /// Unique session identifier
        /// </summary>
        public SessionId SessionId { get; private set; }

        /// <summary>
        /// Player who owns this session
        /// </summary>
        public PlayerId PlayerId { get; private set; }

        /// <summary>
        /// Type of game being played
        /// </summary>
        public GameType GameType { get; private set; }

        /// <summary>
        /// Current session score (read-only public access)
        /// </summary>
        public int Score => _score;

        /// <summary>
        /// When the session started
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// When the session ended (null if still active)
        /// </summary>
        public DateTime? EndTime => _endTime;

        /// <summary>
        /// Whether the session completed successfully
        /// </summary>
        public bool CompletedSuccessfully { get; private set; }

        /// <summary>
        /// Game-specific data storage (readonly access)
        /// </summary>
        public IReadOnlyDictionary<string, object> GameSpecificData => _gameSpecificData.AsReadOnly();

        // Calculated properties
        /// <summary>
        /// Session duration (calculated property)
        /// </summary>
        public TimeSpan Duration => 
            (_endTime ?? DateTime.UtcNow) - StartTime;

        /// <summary>
        /// Whether the session is currently active
        /// </summary>
        public bool IsActive => _endTime == null;

        /// <summary>
        /// Maximum allowed session duration (business rule)
        /// </summary>
        public static readonly TimeSpan MaxSessionDuration = TimeSpan.FromMinutes(60);

        // Private constructor for ORM
        private GameSession() 
        {
            _gameSpecificData = new Dictionary<string, object>();
        }

        /// <summary>
        /// Creates a new game session with domain validation
        /// </summary>
        public static GameSession Create(PlayerId playerId, GameType gameType)
        {
            var session = new GameSession
            {
                SessionId = SessionId.CreateNew(),
                PlayerId = playerId,
                GameType = gameType,
                StartTime = DateTime.UtcNow,
                _score = 0,
                CompletedSuccessfully = false
            };

            // Raise domain event
            session.AddDomainEvent(new GameSessionStartedDomainEvent(
                session.SessionId, session.PlayerId, session.GameType));

            return session;
        }

        /// <summary>
        /// Updates the session score with business validation
        /// </summary>
        /// <param name="additionalScore">Score to add (can be negative)</param>
        public void UpdateScore(int additionalScore)
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot update score of completed session");

            // Business rule: Score cannot go below zero
            _score = Math.Max(0, _score + additionalScore);
            MarkAsModified();
        }

        /// <summary>
        /// Sets the final score for the session
        /// </summary>
        /// <param name="finalScore">Final score achieved</param>
        public void SetFinalScore(int finalScore)
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot set final score of completed session");

            _score = Math.Max(0, finalScore);
            MarkAsModified();
        }

        /// <summary>
        /// Ends the session with completion status
        /// </summary>
        /// <param name="completedSuccessfully">Whether player completed successfully</param>
        public void End(bool completedSuccessfully)
        {
            if (!IsActive)
                throw new InvalidOperationException("Session is already ended");

            _endTime = DateTime.UtcNow;
            CompletedSuccessfully = completedSuccessfully;
            MarkAsModified();

            // Business rule: Validate session duration
            if (Duration > MaxSessionDuration)
            {
                // Could apply penalties or mark as suspicious
                CompletedSuccessfully = false;
            }

            // Raise domain event
            AddDomainEvent(new GameSessionEndedDomainEvent(
                SessionId, PlayerId, GameType, _score, CompletedSuccessfully, Duration));
        }

        /// <summary>
        /// Adds or updates game-specific data
        /// </summary>
        /// <param name="key">Data key</param>
        /// <param name="value">Data value</param>
        public void SetGameData(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Game data key cannot be empty", nameof(key));

            if (!IsActive)
                throw new InvalidOperationException("Cannot update game data of completed session");

            _gameSpecificData[key] = value;
            MarkAsModified();
        }

        /// <summary>
        /// Gets game-specific data by key
        /// </summary>
        /// <typeparam name="T">Expected data type</typeparam>
        /// <param name="key">Data key</param>
        /// <returns>Data value or default if not found</returns>
        public T? GetGameData<T>(string key)
        {
            if (_gameSpecificData.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;
            
            return default;
        }

        /// <summary>
        /// Business rule: Check if session qualifies for scoring
        /// </summary>
        public bool QualifiesForScoring()
        {
            return Duration >= TimeSpan.FromSeconds(30) && // Minimum play time
                   Duration <= MaxSessionDuration &&        // Not too long
                   _score > 0;                              // Some score achieved
        }

        /// <summary>
        /// Business rule: Calculate performance rating (0-5 stars)
        /// </summary>
        public int CalculatePerformanceRating()
        {
            if (!CompletedSuccessfully || _score == 0)
                return 0;

            // Game-specific rating logic
            return GameType switch
            {
                GameType.DeployTheCat => CalculateDeployTheCatRating(),
                GameType.GitBlaster => CalculateGitBlasterRating(),
                _ => 0
            };
        }

        private int CalculateDeployTheCatRating()
        {
            // Rating based on successful deploys and speed
            var successfulDeploys = GetGameData<int>("SuccessfulDeploys");
            var catInterventions = GetGameData<int>("CatInterventions");
            
            if (successfulDeploys == 0) return 1;
            
            var successRate = (double)successfulDeploys / (successfulDeploys + catInterventions);
            var speedBonus = Duration.TotalMinutes < 5 ? 1 : 0;
            
            return successRate switch
            {
                >= 0.9 => 5,
                >= 0.7 => 4 + speedBonus,
                >= 0.5 => 3 + speedBonus,
                >= 0.3 => 2,
                _ => 1
            };
        }

        private int CalculateGitBlasterRating()
        {
            // Rating based on accuracy and command variety
            var accuracy = GetGameData<float>("AverageAccuracy");
            var commandsUsed = GetGameData<int>("UniqueCommandsUsed");
            
            if (accuracy == 0) return 1;
            
            var varietyBonus = commandsUsed >= 5 ? 1 : 0;
            
            return accuracy switch
            {
                >= 0.9f => 5,
                >= 0.8f => 4 + varietyBonus,
                >= 0.6f => 3 + varietyBonus,
                >= 0.4f => 2,
                _ => 1
            };
        }

        /// <summary>
        /// Business rule: Check if session should be flagged for review
        /// (unusually high scores, very short duration, etc.)
        /// </summary>
        public bool ShouldFlagForReview()
        {
            // Suspiciously high score for short duration
            if (Duration.TotalMinutes < 1 && _score > 1000)
                return true;

            // Impossibly perfect performance
            if (GameType == GameType.GitBlaster)
            {
                var accuracy = GetGameData<float>("AverageAccuracy");
                var responseTime = GetGameData<double>("AverageResponseTimeMs");
                if (accuracy >= 1.0f && responseTime < 100) // Perfect accuracy with superhuman speed
                    return true;
            }

            return false;
        }
    }
}