// src/Core/GamingLibrary.Application/Features/Players/DTOs/PlayerResponse.cs
// Purpose: Player response DTO for API
using GamingLibrary.Domain.ValueObjects;

namespace GamingLibrary.Application.Features.Players.DTOs
{
    /// <summary>
    /// Player response DTO for API responses
    /// </summary>
    public class PlayerResponse
    {
        public string PlayerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalScore { get; set; }
        public int GamesPlayed { get; set; }
        public DateTime LastPlayedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public double AverageScorePerGame { get; set; }
    }
}