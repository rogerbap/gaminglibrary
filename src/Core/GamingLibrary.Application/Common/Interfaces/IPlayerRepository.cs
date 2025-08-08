// src/Core/GamingLibrary.Application/Common/Interfaces/IPlayerRepository.cs
// Purpose: Player-specific repository interface
using GamingLibrary.Domain.Entities;
using GamingLibrary.Domain.ValueObjects;

namespace GamingLibrary.Application.Common.Interfaces
{
    /// <summary>
    /// Repository interface for Player aggregate
    /// </summary>
    public interface IPlayerRepository : IRepository<Player>
    {
        Task<Player?> GetByPlayerIdAsync(PlayerId playerId);
        Task<Player?> GetByEmailAsync(Email email);
        Task<IEnumerable<Player>> GetTopPlayersByScoreAsync(int count);
        Task<IEnumerable<Player>> GetActivePlayersAsync();
        Task<bool> EmailExistsAsync(Email email);
    }
}