// src/Core/GamingLibrary.Application/Features/Players/Commands/CreatePlayer/CreatePlayerCommandHandler.cs
// Purpose: Handler for creating new players
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Application.Common.Models;
using GamingLibrary.Application.Features.Players.DTOs;
using GamingLibrary.Domain.Entities;
using GamingLibrary.Domain.ValueObjects;
using GamingLibrary.Domain.Exceptions;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace GamingLibrary.Application.Features.Players.Commands.CreatePlayer
{
    /// <summary>
    /// Handles player creation with business logic validation
    /// </summary>
    public class CreatePlayerCommandHandler : ICommandHandler<CreatePlayerCommand, Result<PlayerResponse>>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreatePlayerCommandHandler> _logger;

        public CreatePlayerCommandHandler(
            IPlayerRepository playerRepository,
            IMapper mapper,
            ILogger<CreatePlayerCommandHandler> logger)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PlayerResponse>> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating new player with email: {Email}", request.Email);

                // Create value objects (with validation)
                var playerName = PlayerName.Create(request.Name);
                var email = Email.Create(request.Email);

                // Business rule: Check if email already exists
                if (await _playerRepository.EmailExistsAsync(email))
                {
                    _logger.LogWarning("Attempted to create player with existing email: {Email}", request.Email);
                    return Result.Failure<PlayerResponse>("A player with this email already exists");
                }

                // Create domain entity
                var player = Player.Create(playerName, email);

                // Persist to database
                var savedPlayer = await _playerRepository.AddAsync(player);

                // Map to response DTO
                var response = _mapper.Map<PlayerResponse>(savedPlayer);

                _logger.LogInformation("Successfully created player: {PlayerId}", savedPlayer.PlayerId);

                return Result.Success(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating player: {Error}", ex.Message);
                return Result.Failure<PlayerResponse>(ex.Message);
            }
            catch (PlayerDomainException ex)
            {
                _logger.LogWarning(ex, "Domain error creating player: {Error}", ex.Message);
                return Result.Failure<PlayerResponse>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating player");
                return Result.Failure<PlayerResponse>("An unexpected error occurred while creating the player");
            }
        }
    }
}