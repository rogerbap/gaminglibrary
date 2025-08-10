// src/Core/GamingLibrary.Application/Features/GameSessions/Commands/EndGameSession/EndGameSessionCommand.cs
// Purpose: Command to end a game session
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Application.Common.Models;
using GamingLibrary.Application.Features.GameSessions.DTOs;

namespace GamingLibrary.Application.Features.GameSessions.Commands.EndGameSession
{
    /// <summary>
    /// Command to end an active game session
    /// </summary>
    public record EndGameSessionCommand(
        string SessionId, 
        int FinalScore, 
        bool CompletedSuccessfully,
        Dictionary<string, object>? FinalGameData = null
    ) : ICommand<Result<GameSessionResponse>>;
}