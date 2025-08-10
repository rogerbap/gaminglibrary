// src/Core/GamingLibrary.Application/Features/GameSessions/Queries/GetPlayerSessions/GetPlayerSessionsQuery.cs
// Purpose: Query to get all sessions for a player
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Application.Common.Models;
using GamingLibrary.Application.Features.GameSessions.DTOs;
using GamingLibrary.Domain.Enums;

namespace GamingLibrary.Application.Features.GameSessions.Queries.GetPlayerSessions
{
    /// <summary>
    /// Query to retrieve all sessions for a specific player
    /// </summary>
    public record GetPlayerSessionsQuery(
        string PlayerId,
        GameType? GameType = null,
        int? Limit = null
    ) : IQuery<Result<IEnumerable<GameSessionResponse>>>;
}