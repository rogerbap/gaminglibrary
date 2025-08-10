// src/Core/GamingLibrary.Application/Common/Mappings/GameSessionMappingProfile.cs
// Purpose: AutoMapper configuration for GameSession mappings
using AutoMapper;
using GamingLibrary.Application.Features.GameSessions.DTOs;
using GamingLibrary.Domain.Entities;

namespace GamingLibrary.Application.Common.Mappings
{
    /// <summary>
    /// AutoMapper profile for GameSession entity mappings
    /// </summary>
    public class GameSessionMappingProfile : Profile
    {
        public GameSessionMappingProfile()
        {
            CreateMap<GameSession, GameSessionResponse>()
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.SessionId.Value))
                .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId.Value))
                .ForMember(dest => dest.GameType, opt => opt.MapFrom(src => src.GameType))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.CompletedSuccessfully, opt => opt.MapFrom(src => src.CompletedSuccessfully))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.GameSpecificData, opt => opt.MapFrom(src => ConvertGameData(src.GameSpecificData)))
                .ForMember(dest => dest.PerformanceRating, opt => opt.Ignore()); // Calculated separately

            // Helper method to convert readonly dictionary to regular dictionary
            CreateMap<IReadOnlyDictionary<string, object>, Dictionary<string, object>>()
                .ConvertUsing(src => src.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        private static Dictionary<string, object> ConvertGameData(IReadOnlyDictionary<string, object> gameData)
        {
            return gameData?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>();
        }
    }
}
