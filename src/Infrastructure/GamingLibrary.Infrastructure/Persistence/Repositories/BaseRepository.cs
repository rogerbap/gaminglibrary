// src/Infrastructure/GamingLibrary.Infrastructure/Persistence/Repositories/BaseRepository.cs
// Purpose: Base repository implementation with common MongoDB operations
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Domain.Common;
using MongoDB.Driver;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace GamingLibrary.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Base repository implementation providing common MongoDB operations.
    /// Implements the Repository pattern with MongoDB-specific optimizations.
    /// </summary>
    /// <typeparam name="T">Entity type that extends BaseEntity</typeparam>
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;
        protected readonly ILogger<BaseRepository<T>> _logger;

        protected BaseRepository(IMongoDatabase database, string collectionName, ILogger<BaseRepository<T>> logger)
        {
            _collection = database.GetCollection<T>(collectionName);
            _logger = logger;
        }

        public virtual async Task<T?> GetByIdAsync(string id)
        {
            try
            {
                _logger.LogDebug("Getting {EntityType} by ID: {Id}", typeof(T).Name, id);
                
                var filter = Builders<T>.Filter.Eq(e => e.Id, id);
                var result = await _collection.Find(filter).FirstOrDefaultAsync();
                
                _logger.LogDebug("Retrieved {EntityType}: {Found}", typeof(T).Name, result != null ? "Found" : "Not Found");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting {EntityType} by ID: {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                _logger.LogDebug("Getting all {EntityType} entities", typeof(T).Name);
                
                var result = await _collection.Find(_ => true).ToListAsync();
                
                _logger.LogDebug("Retrieved {Count} {EntityType} entities", result.Count, typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all {EntityType} entities", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                _logger.LogDebug("Finding {EntityType} entities with predicate", typeof(T).Name);
                
                var result = await _collection.Find(predicate).ToListAsync();
                
                _logger.LogDebug("Found {Count} {EntityType} entities", result.Count, typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding {EntityType} entities", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            try
            {
                _logger.LogDebug("Adding new {EntityType}", typeof(T).Name);
                
                // Generate ID if not set
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.GetType().GetProperty("Id")?.SetValue(entity, MongoDB.Bson.ObjectId.GenerateNewId().ToString());
                }

                await _collection.InsertOneAsync(entity);
                
                _logger.LogInformation("Successfully added {EntityType} with ID: {Id}", typeof(T).Name, entity.Id);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            try
            {
                _logger.LogDebug("Updating {EntityType} with ID: {Id}", typeof(T).Name, entity.Id);
                
                var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
                var result = await _collection.ReplaceOneAsync(filter, entity);
                
                if (result.MatchedCount == 0)
                {
                    _logger.LogWarning("No {EntityType} found with ID: {Id} for update", typeof(T).Name, entity.Id);
                    throw new InvalidOperationException($"Entity with ID {entity.Id} not found for update");
                }
                
                _logger.LogDebug("Successfully updated {EntityType} with ID: {Id}", typeof(T).Name, entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating {EntityType} with ID: {Id}", typeof(T).Name, entity.Id);
                throw;
            }
        }

        public virtual async Task DeleteAsync(string id)
        {
            try
            {
                _logger.LogDebug("Deleting {EntityType} with ID: {Id}", typeof(T).Name, id);
                
                var filter = Builders<T>.Filter.Eq(e => e.Id, id);
                var result = await _collection.DeleteOneAsync(filter);
                
                if (result.DeletedCount == 0)
                {
                    _logger.LogWarning("No {EntityType} found with ID: {Id} for deletion", typeof(T).Name, id);
                    throw new InvalidOperationException($"Entity with ID {id} not found for deletion");
                }
                
                _logger.LogInformation("Successfully deleted {EntityType} with ID: {Id}", typeof(T).Name, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {EntityType} with ID: {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                _logger.LogDebug("Checking existence of {EntityType} with predicate", typeof(T).Name);
                
                var count = await _collection.CountDocumentsAsync(predicate, new CountOptions { Limit = 1 });
                
                _logger.LogDebug("{EntityType} exists: {Exists}", typeof(T).Name, count > 0);
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<long> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            try
            {
                _logger.LogDebug("Counting {EntityType} entities", typeof(T).Name);
                
                var count = predicate != null 
                    ? await _collection.CountDocumentsAsync(predicate)
                    : await _collection.CountDocumentsAsync(_ => true);
                
                _logger.LogDebug("Count of {EntityType}: {Count}", typeof(T).Name, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting {EntityType} entities", typeof(T).Name);
                throw;
            }
        }
    }
}