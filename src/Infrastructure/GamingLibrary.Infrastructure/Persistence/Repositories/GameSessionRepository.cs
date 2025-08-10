// src/Infrastructure/GamingLibrary.Infrastructure/Persistence/Repositories/GameSessionRepository.cs
// Purpose: MongoDB implementation of IGameSessionRepository
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Domain.Entities;
using GamingLibrary.Domain.ValueObjects;
using GamingLibrary.Domain.Enums;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace GamingLibrary.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// MongoDB implementation of the GameSession repository.
    /// Provides GameSession-specific query operations with optimized MongoDB queries.
    /// </summary>
    public class GameSessionRepository : BaseRepository<GameSession>, IGameSessionRepository
    {
        public GameSessionRepository(IMongoDatabase database, ILogger<GameSessionRepository> logger)
            : base(database, "gameSessions", logger)
        {
            CreateIndexes();
        }

        public async Task<GameSession?> GetBySessionIdAsync(SessionId sessionId)
        {
            try
            {
                _logger.LogDebug("Getting game session by SessionId: {SessionId}", sessionId.Value);
                
                var filter = Builders<GameSession>.Filter.Eq("sessionId", sessionId.Value);
                var result = await _collection.Find(filter).FirstOrDefaultAsync();
                
                _logger.LogDebug("Retrieved game session: {Found}", result != null ? "Found" : "Not Found");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game session by SessionId: {SessionId}", sessionId.Value);
                throw;
            }
        }

        public async Task<IEnumerable<GameSession>> GetByPlayerIdAsync(PlayerId playerId)
        {
            try
            {
                _logger.LogDebug("Getting game sessions for player: {PlayerId}", playerId.Value);
                
                var filter = Builders<GameSession>.Filter.Eq("playerId", playerId.Value);
                var sort = Builders<GameSession>.Sort.Descending(gs => gs.StartTime);
                var result = await _collection.Find(filter).Sort(sort).ToListAsync();
                
                _logger.LogDebug("Retrieved {Count} game sessions for player", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game sessions for player: {PlayerId}", playerId.Value);
                throw;
            }
        }

        public async Task<IEnumerable<GameSession>> GetByPlayerAndGameTypeAsync(PlayerId playerId, GameType gameType)
        {
            try
            {
                _logger.LogDebug("Getting {GameType} sessions for player: {PlayerId}", gameType, playerId.Value);
                
                var filter = Builders<GameSession>.Filter.And(
                    Builders<GameSession>.Filter.Eq("playerId", playerId.Value),
                    Builders<GameSession>.Filter.Eq(gs => gs.GameType, gameType)
                );
                
                var sort = Builders<GameSession>.Sort.Descending(gs => gs.StartTime);
                var result = await _collection.Find(filter).Sort(sort).ToListAsync();
                
                _logger.LogDebug("Retrieved {Count} {GameType} sessions for player", result.Count, gameType);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {GameType} sessions for player: {PlayerId}", gameType, playerId.Value);
                throw;
            }
        }

        public async Task<IEnumerable<GameSession>> GetActiveSessionsAsync()
        {
            try
            {
                _logger.LogDebug("Getting all active game sessions");
                
                var filter = Builders<GameSession>.Filter.Eq("endTime", BsonNull.Value);
                var result = await _collection.Find(filter).ToListAsync();
                
                _logger.LogDebug("Retrieved {Count} active game sessions", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active game sessions");
                throw;
            }
        }

        public async Task<IEnumerable<GameSession>> GetCompletedSessionsAsync(DateTime since)
        {
            try
            {
                _logger.LogDebug("Getting completed game sessions since: {Since}", since);
                
                var filter = Builders<GameSession>.Filter.And(
                    Builders<GameSession>.Filter.Ne("endTime", BsonNull.Value),
                    Builders<GameSession>.Filter.Gte(gs => gs.StartTime, since)
                );
                
                var sort = Builders<GameSession>.Sort.Descending(gs => gs.StartTime);
                var result = await _collection.Find(filter).Sort(sort).ToListAsync();
                
                _logger.LogDebug("Retrieved {Count} completed game sessions since {Since}", result.Count, since);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting completed game sessions since: {Since}", since);
                throw;
            }
        }

        private void CreateIndexes()
        {
            try
            {
                _logger.LogDebug("Creating indexes for GameSession collection");

                // Index on sessionId for fast lookups
                var sessionIdIndex = Builders<GameSession>.IndexKeys.Ascending("sessionId");
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<GameSession>(sessionIdIndex,
                    new CreateIndexOptions { Unique = true, Name = "sessionId_unique" }));

                // Index on playerId for player session queries
                var playerIdIndex = Builders<GameSession>.IndexKeys.Ascending("playerId");
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<GameSession>(playerIdIndex,
                    new CreateIndexOptions { Name = "playerId_sessions" }));

                // Compound index on playerId and gameType
                var playerGameTypeIndex = Builders<GameSession>.IndexKeys
                    .Ascending("playerId")
                    .Ascending(gs => gs.GameType);
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<GameSession>(playerGameTypeIndex,
                    new CreateIndexOptions { Name = "player_gametype" }));

                // Index on startTime for chronological queries
                var startTimeIndex = Builders<GameSession>.IndexKeys.Descending(gs => gs.StartTime);
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<GameSession>(startTimeIndex,
                    new CreateIndexOptions { Name = "start_time_desc" }));

                // Index on endTime for active session queries (null values)
                var endTimeIndex = Builders<GameSession>.IndexKeys.Ascending("endTime");
                _collection.Indexes.CreateOneAsync(new CreateIndexModel<GameSession>(endTimeIndex,
                    new CreateIndexOptions { Name = "end_time_active", Sparse = true }));

                _logger.LogDebug("Successfully created indexes for GameSession collection");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating indexes for GameSession collection");
            }
        }
    }
}