// src/Core/GamingLibrary.Application/Features/GameSessions/Queries/GetSessionById/GetSessionByIdQuery.cs
// Purpose: Query to get a session by ID
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Application.Common.Models;
using GamingLibrary.Application.Features.GameSessions.DTOs;

namespace GamingLibrary.Application.Features.GameSessions.Queries.GetSessionById
{
    /// <summary>
    /// Query to retrieve a game session by its ID
    /// </summary>
    public record GetSessionByIdQuery(string SessionId) : IQuery<Result<GameSessionResponse>>;
}
