// src/Core/GamingLibrary.Application/Features/Players/Commands/CreatePlayer/CreatePlayerCommandValidator.cs
// Purpose: FluentValidation validator for CreatePlayerCommand
using FluentValidation;

namespace GamingLibrary.Application.Features.Players.Commands.CreatePlayer
{
    /// <summary>
    /// Validates CreatePlayerCommand input
    /// </summary>
    public class CreatePlayerCommandValidator : AbstractValidator<CreatePlayerCommand>
    {
        public CreatePlayerCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Player name is required")
                .Length(2, 50).WithMessage("Player name must be between 2 and 50 characters")
                .Matches(@"^[a-zA-Z0-9\s\-_\.]+$").WithMessage("Player name contains invalid characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email is too long");
        }
    }
}