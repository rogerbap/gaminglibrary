// src/Core/GamingLibrary.Application/Features/GameSessions/Commands/StartGameSession/StartGameSessionCommand.cs
// Purpose: Command to start a new game session
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Application.Common.Models;
using GamingLibrary.Application.Features.GameSessions.DTOs;
using GamingLibrary.Domain.Enums;

namespace GamingLibrary.Application.Features.GameSessions.Commands.StartGameSession
{
    /// <summary>
    /// Command to start a new game session
    /// </summary>
    public record StartGameSessionCommand(string PlayerId, GameType GameType) : ICommand<Result<GameSessionResponse>>;
}