// src/Core/GamingLibrary.Application/Features/GameSessions/Commands/StartGameSession/StartGameSessionCommandHandler.cs
// Purpose: Handler for starting game sessions
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Application.Common.Models;
using GamingLibrary.Application.Features.GameSessions.DTOs;
using GamingLibrary.Domain.Entities;
using GamingLibrary.Domain.ValueObjects;
using GamingLibrary.Domain.Exceptions;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace GamingLibrary.Application.Features.GameSessions.Commands.StartGameSession
{
    /// <summary>
    /// Handles game session creation with business validation
    /// </summary>
    public class StartGameSessionCommandHandler : ICommandHandler<StartGameSessionCommand, Result<GameSessionResponse>>
    {
        private readonly IGameSessionRepository _sessionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<StartGameSessionCommandHandler> _logger;

        public StartGameSessionCommandHandler(
            IGameSessionRepository sessionRepository,
            IPlayerRepository playerRepository,
            IMapper mapper,
            ILogger<StartGameSessionCommandHandler> logger)
        {
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<GameSessionResponse>> Handle(StartGameSessionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting game session: {GameType} for player: {PlayerId}", 
                    request.GameType, request.PlayerId);

                // Validate player exists and is active
                var playerId = PlayerId.Create(request.PlayerId);
                var player = await _playerRepository.GetByPlayerIdAsync(playerId);
                
                if (player == null)
                {
                    _logger.LogWarning("Attempted to start session for non-existent player: {PlayerId}", request.PlayerId);
                    return Result.Failure<GameSessionResponse>($"Player '{request.PlayerId}' not found");
                }

                if (!player.IsActive)
                {
                    _logger.LogWarning("Attempted to start session for inactive player: {PlayerId}", request.PlayerId);
                    return Result.Failure<GameSessionResponse>("Player account is inactive");
                }

                // Business rule: Check for existing active sessions
                var activeSessions = await _sessionRepository.GetByPlayerIdAsync(playerId);
                var existingActiveSession = activeSessions.FirstOrDefault(s => s.IsActive);
                
                if (existingActiveSession != null)
                {
                    _logger.LogWarning("Player {PlayerId} already has active session: {SessionId}", 
                        request.PlayerId, existingActiveSession.SessionId);
                    return Result.Failure<GameSessionResponse>(
                        $"Player already has an active session: {existingActiveSession.SessionId}");
                }

                // Create new game session
                var session = GameSession.Create(playerId, request.GameType);

                // Update player's last played timestamp
                player.RecordGameStart();

                // Persist changes
                var savedSession = await _sessionRepository.AddAsync(session);
                await _playerRepository.UpdateAsync(player);

                // Map to response DTO
                var response = _mapper.Map<GameSessionResponse>(savedSession);

                _logger.LogInformation("Successfully started game session: {SessionId}", savedSession.SessionId);

                return Result.Success(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error starting game session: {Error}", ex.Message);
                return Result.Failure<GameSessionResponse>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error starting game session");
                return Result.Failure<GameSessionResponse>("An unexpected error occurred while starting the game session");
            }
        }
    }
}