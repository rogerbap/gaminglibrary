// src/Core/GamingLibrary.Application/Features/GameSessions/Commands/UpdateSessionData/UpdateSessionDataCommand.cs
// Purpose: Command to update game-specific data during gameplay
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Application.Common.Models;

namespace GamingLibrary.Application.Features.GameSessions.Commands.UpdateSessionData
{
    /// <summary>
    /// Command to update game-specific data during active gameplay
    /// </summary>
    public record UpdateSessionDataCommand(
        string SessionId,
        Dictionary<string, object> GameData
    ) : ICommand<Result>;
}