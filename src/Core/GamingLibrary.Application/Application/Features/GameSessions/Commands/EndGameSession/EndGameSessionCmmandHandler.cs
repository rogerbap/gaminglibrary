// src/Core/GamingLibrary.Application/Features/GameSessions/Commands/EndGameSession/EndGameSessionCommandHandler.cs
// Purpose: Handler for ending game sessions
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Application.Common.Models;
using GamingLibrary.Application.Features.GameSessions.DTOs;
using GamingLibrary.Domain.ValueObjects;
using GamingLibrary.Domain.Exceptions;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace GamingLibrary.Application.Features.GameSessions.Commands.EndGameSession
{
    /// <summary>
    /// Handles game session completion with score updates
    /// </summary>
    public class EndGameSessionCommandHandler : ICommandHandler<EndGameSessionCommand, Result<GameSessionResponse>>
    {
        private readonly IGameSessionRepository _sessionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EndGameSessionCommandHandler> _logger;

        public EndGameSessionCommandHandler(
            IGameSessionRepository sessionRepository,
            IPlayerRepository playerRepository,
            IMapper mapper,
            ILogger<EndGameSessionCommandHandler> logger)
        {
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<GameSessionResponse>> Handle(EndGameSessionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Ending game session: {SessionId} with score: {Score}", 
                    request.SessionId, request.FinalScore);

                // Find and validate session
                var sessionId = SessionId.Create(request.SessionId);
                var session = await _sessionRepository.GetBySessionIdAsync(sessionId);

                if (session == null)
                {
                    _logger.LogWarning("Attempted to end non-existent session: {SessionId}", request.SessionId);
                    return Result.Failure<GameSessionResponse>($"Game session '{request.SessionId}' not found");
                }

                if (!session.IsActive)
                {
                    _logger.LogWarning("Attempted to end already completed session: {SessionId}", request.SessionId);
                    return Result.Failure<GameSessionResponse>($"Game session '{request.SessionId}' has already ended");
                }

                // Get player for score update
                var player = await _playerRepository.GetByPlayerIdAsync(session.PlayerId);
                if (player == null)
                {
                    _logger.LogError("Session {SessionId} references non-existent player: {PlayerId}", 
                        request.SessionId, session.PlayerId);
                    return Result.Failure<GameSessionResponse>("Player not found for this session");
                }

                // Store final game-specific data if provided
                if (request.FinalGameData != null)
                {
                    foreach (var kvp in request.FinalGameData)
                    {
                        session.SetGameData(kvp.Key, kvp.Value);
                    }
                }

                // Set final score and end session
                session.SetFinalScore(request.FinalScore);
                session.End(request.CompletedSuccessfully);

                // Update player score and stats
                if (session.QualifiesForScoring())
                {
                    player.UpdateScore(request.FinalScore, request.CompletedSuccessfully);
                    _logger.LogInformation("Updated player {PlayerId} score by {Score} points", 
                        session.PlayerId, request.FinalScore);
                }

                // Persist changes
                await _sessionRepository.UpdateAsync(session);
                await _playerRepository.UpdateAsync(player);

                // Map to response DTO
                var response = _mapper.Map<GameSessionResponse>(session);
                response.PerformanceRating = session.CalculatePerformanceRating();

                _logger.LogInformation("Successfully ended game session: {SessionId} | Duration: {Duration} | Rating: {Rating}", 
                    session.SessionId, session.Duration, response.PerformanceRating);

                return Result.Success(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error ending game session: {Error}", ex.Message);
                return Result.Failure<GameSessionResponse>(ex.Message);
            }
            catch (GameSessionDomainException ex)
            {
                _logger.LogWarning(ex, "Domain error ending game session: {Error}", ex.Message);
                return Result.Failure<GameSessionResponse>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error ending game session: {SessionId}", request.SessionId);
                return Result.Failure<GameSessionResponse>("An unexpected error occurred while ending the game session");
            }
        }
    }
}