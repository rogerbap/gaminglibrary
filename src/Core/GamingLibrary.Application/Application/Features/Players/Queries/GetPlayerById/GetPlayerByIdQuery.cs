// src/Core/GamingLibrary.Application/Features/Players/Queries/GetPlayerById/GetPlayerByIdQuery.cs
// Purpose: Query to get a player by ID
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Application.Common.Models;
using GamingLibrary.Application.Features.Players.DTOs;

namespace GamingLibrary.Application.Features.Players.Queries.GetPlayerById
{
    /// <summary>
    /// Query to retrieve a player by their ID
    /// </summary>
    public record GetPlayerByIdQuery(string PlayerId) : IQuery<Result<PlayerResponse>>;
}
