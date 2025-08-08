// src/Core/GamingLibrary.Application/Features/Players/Commands/CreatePlayer/CreatePlayerCommand.cs
// Purpose: Command to create a new player
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Application.Common.Models;
using GamingLibrary.Application.Features.Players.DTOs;

namespace GamingLibrary.Application.Features.Players.Commands.CreatePlayer
{
    /// <summary>
    /// Command to create a new player account
    /// </summary>
    public record CreatePlayerCommand(string Name, string Email) : ICommand<Result<PlayerResponse>>;
}
