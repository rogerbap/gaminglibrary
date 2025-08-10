// src/Core/GamingLibrary.Application/Common/Mappings/PlayerMappingProfile.cs
// Purpose: AutoMapper configuration for Player mappings
using AutoMapper;
using GamingLibrary.Application.Features.Players.DTOs;
using GamingLibrary.Domain.Entities;

namespace GamingLibrary.Application.Common.Mappings
{
    /// <summary>
    /// AutoMapper profile for Player entity mappings
    /// Handles conversion between domain entities and DTOs
    /// </summary>
    public class PlayerMappingProfile : Profile
    {
        public PlayerMappingProfile()
        {
            CreateMap<Player, PlayerResponse>()
                .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId.Value))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
                .ForMember(dest => dest.TotalScore, opt => opt.MapFrom(src => src.TotalScore))
                .ForMember(dest => dest.GamesPlayed, opt => opt.MapFrom(src => src.GamesPlayed))
                .ForMember(dest => dest.LastPlayedAt, opt => opt.MapFrom(src => src.LastPlayedAt))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.AverageScorePerGame, opt => opt.MapFrom(src => src.AverageScorePerGame()));
        }
    }
}