// src/Core/GamingLibrary.Application/Features/GameSessions/Commands/StartGameSession/StartGameSessionCommandValidator.cs
// Purpose: Validator for StartGameSessionCommand
using FluentValidation;
using GamingLibrary.Domain.Enums;

namespace GamingLibrary.Application.Features.GameSessions.Commands.StartGameSession
{
    /// <summary>
    /// Validates StartGameSessionCommand input
    /// </summary>
    public class StartGameSessionCommandValidator : AbstractValidator<StartGameSessionCommand>
    {
        public StartGameSessionCommandValidator()
        {
            RuleFor(x => x.PlayerId)
                .NotEmpty().WithMessage("Player ID is required")
                .Must(BeValidGuid).WithMessage("Player ID must be a valid GUID");

            RuleFor(x => x.GameType)
                .IsInEnum().WithMessage("Invalid game type")
                .Must(BeValidGameType).WithMessage("Game type is not currently supported");
        }

        private static bool BeValidGuid(string playerId)
        {
            return Guid.TryParse(playerId, out _);
        }

        private static bool BeValidGameType(GameType gameType)
        {
            return gameType == GameType.DeployTheCat || gameType == GameType.GitBlaster;
        }
    }
}