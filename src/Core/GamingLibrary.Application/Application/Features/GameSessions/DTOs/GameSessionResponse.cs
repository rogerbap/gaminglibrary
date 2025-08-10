// src/Core/GamingLibrary.Application/Features/GameSessions/DTOs/GameSessionResponse.cs
// Purpose: GameSession response DTO for API
using GamingLibrary.Domain.Enums;

namespace GamingLibrary.Application.Features.GameSessions.DTOs
{
    /// <summary>
    /// GameSession response DTO for API responses
    /// </summary>
    public class GameSessionResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public GameType GameType { get; set; }
        public int Score { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool CompletedSuccessfully { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<string, object> GameSpecificData { get; set; } = new();
        public int PerformanceRating { get; set; }
    }
}
