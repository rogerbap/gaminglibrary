// src/Infrastructure/GamingLibrary.Infrastructure/Persistence/Configurations/PlayerConfiguration.cs
// Purpose: MongoDB configuration and mapping for Player entity
using GamingLibrary.Domain.Entities;
using GamingLibrary.Domain.ValueObjects;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace GamingLibrary.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// MongoDB mapping configuration for Player entity.
    /// Configures how domain entities are stored in MongoDB.
    /// </summary>
    public static class PlayerConfiguration
    {
        public static void Configure()
        {
            // Only configure if not already configured
            if (BsonClassMap.IsClassMapRegistered(typeof(Player)))
                return;

            BsonClassMap.RegisterClassMap<Player>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                
                // Map the MongoDB _id field to our Id property
                cm.MapIdProperty(p => p.Id);
                
                // Map value objects to their string representations
                cm.MapProperty(p => p.PlayerId)
                    .SetSerializer(new StringSerializer())
                    .SetElementName("playerId");
                    
                cm.MapProperty(p => p.Name)
                    .SetSerializer(new StringSerializer())
                    .SetElementName("name");
                    
                cm.MapProperty(p => p.Email)
                    .SetSerializer(new StringSerializer())
                    .SetElementName("email");

                // Map private fields for encapsulation
                cm.MapField("_totalScore").SetElementName("totalScore");
                cm.MapField("_gamesPlayed").SetElementName("gamesPlayed");
                
                // Timestamps
                cm.MapProperty(p => p.CreatedAt).SetElementName("createdAt");
                cm.MapProperty(p => p.UpdatedAt).SetElementName("updatedAt");
                cm.MapProperty(p => p.LastPlayedAt).SetElementName("lastPlayedAt");
                cm.MapProperty(p => p.IsActive).SetElementName("isActive");

                // Ignore domain events for persistence
                cm.UnmapProperty(p => p.DomainEvents);
            });

            // Configure value object serialization
            BsonClassMap.RegisterClassMap<PlayerId>(cm =>
            {
                cm.MapProperty(p => p.Value).SetElementName("value");
            });

            BsonClassMap.RegisterClassMap<PlayerName>(cm =>
            {
                cm.MapProperty(p => p.Value).SetElementName("value");
            });

            BsonClassMap.RegisterClassMap<Email>(cm =>
            {
                cm.MapProperty(p => p.Value).SetElementName("value");
            });
        }
    }
}
