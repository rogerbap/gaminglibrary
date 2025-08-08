// src/Core/GamingLibrary.Application/Common/Interfaces/IGameSessionRepository.cs
// Purpose: GameSession repository interface
using GamingLibrary.Domain.Entities;
using GamingLibrary.Domain.ValueObjects;
using GamingLibrary.Domain.Enums;

namespace GamingLibrary.Application.Common.Interfaces
{
    /// <summary>
    /// Repository interface for GameSession aggregate
    /// </summary>
    public interface IGameSessionRepository : IRepository<GameSession>
    {
        Task<GameSession?> GetBySessionIdAsync(SessionId sessionId);
        Task<IEnumerable<GameSession>> GetByPlayerIdAsync(PlayerId playerId);
        Task<IEnumerable<GameSession>> GetByPlayerAndGameTypeAsync(PlayerId playerId, GameType gameType);
        Task<IEnumerable<GameSession>> GetActiveSessionsAsync();
        Task<IEnumerable<GameSession>> GetCompletedSessionsAsync(DateTime since);
    }
}