// src/Infrastructure/GamingLibrary.Infrastructure/Persistence/Repositories/PlayerRepository.cs
// Purpose: MongoDB implementation of IPlayerRepository
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Domain.Entities;
using GamingLibrary.Domain.ValueObjects;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace GamingLibrary.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// MongoDB implementation of the Player repository.
    /// Provides Player-specific query operations with optimized MongoDB queries.
    /// </summary>
    public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(IMongoDatabase database, ILogger<PlayerRepository> logger)
            : base(database, "players", logger)
        {
            // Create indexes for better query performance
            CreateIndexes();
        }

        public async Task<Player?> GetByPlayerIdAsync(PlayerId playerId)
        {
            try
            {
                _logger.LogDebug("Getting player by PlayerId: {PlayerId}", playerId.Value);
                
                var filter = Builders<Player>.Filter.Eq("playerId", playerId.Value);
                var result = await _collection.Find(filter).FirstOrDefaultAsync();
                
                _logger.LogDebug("Retrieved player: {Found}", result != null ? "Found" : "Not Found");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting player by PlayerId: {PlayerId}", playerId.Value);
                throw;
            }
        }

        public async Task<Player?> GetByEmailAsync(Email email)
        {
            try
            {
                _logger.LogDebug("Getting player by email: {Email}", email.Value);
                
                var filter = Builders<Player>.Filter.Eq("email", email.Value);
                var result = await _collection.Find(filter).FirstOrDefaultAsync();
                
                _logger.LogDebug("Retrieved player by email: {Found}", result != null ? "Found" : "Not Found");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting player by email: {Email}", email.Value);
                throw;
            }
        }

        public async Task<IEnumerable<Player>> GetTopPlayersByScoreAsync(int count)
        {
            try
            {
                _logger.LogDebug("Getting top {Count} players by score", count);
                
                var sort = Builders<Player>.Sort.Descending("totalScore");
                var result = await _collection
                    .Find(p => p.IsActive)
                    .Sort(sort)
                    .Limit(count)
                    .ToListAsync();
                
                _logger.LogDebug("Retrieved {Count} top players", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top players by score");
                throw;
            }
        }

        public async Task<IEnumerable<Player>> GetActivePlayersAsync()
        {
            try
            {
                _logger.LogDebug("Getting all active players");
                
                var filter = Builders<Player>.Filter.Eq(p => p.IsActive, true);
                var result = await _collection.Find(filter).ToListAsync();
                
                _logger.LogDebug("Retrieved {Count} active players", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active players");
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(Email email)
        {
            try
            {
                _logger.LogDebug("Checking if email exists: {Email}", email.Value);
                
                var filter = Builders<Player>.Filter.Eq("email", email.Value);
                var count = await _collection.CountDocumentsAsync(filter, new CountOptions { Limit = 1 });
                
                var exists = count > 0;
                _logger.LogDebug("Email exists: {Exists}", exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if email exists: {Email}", email.Value);
                throw;
            }
        }

        private void CreateIndexes()
        {
            try
            {
                _logger.LogDebug("Creating indexes for Player collection");

                // Index on playerId for fast lookups
                var playerIdIndex = Builders<Player>.IndexKeys.Ascending("playerId");
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<Player>(playerIdIndex, 
                    new CreateIndexOptions { Unique = true, Name = "playerId_unique" }));

                // Index on email for fast lookups and uniqueness
                var emailIndex = Builders<Player>.IndexKeys.Ascending("email");
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<Player>(emailIndex,
                    new CreateIndexOptions { Unique = true, Name = "email_unique" }));

                // Compound index on isActive and totalScore for leaderboard queries
                var leaderboardIndex = Builders<Player>.IndexKeys
                    .Ascending(p => p.IsActive)
                    .Descending("totalScore");
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<Player>(leaderboardIndex,
                    new CreateIndexOptions { Name = "leaderboard_active_score" }));

                // Index on lastPlayedAt for activity queries
                var activityIndex = Builders<Player>.IndexKeys.Descending(p => p.LastPlayedAt);
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<Player>(activityIndex,
                    new CreateIndexOptions { Name = "last_played_activity" }));

                _logger.LogDebug("Successfully created indexes for Player collection");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating indexes for Player collection");
                // Don't throw - indexes are optimization, not critical for functionality
            }
        }
    }
}