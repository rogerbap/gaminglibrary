// src/Core/GamingLibrary.Application/Features/Players/Queries/GetPlayerById/GetPlayerByIdQueryHandler.cs
// Purpose: Handler for getting player by ID
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Application.Common.Models;
using GamingLibrary.Application.Features.Players.DTOs;
using GamingLibrary.Domain.ValueObjects;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace GamingLibrary.Application.Features.Players.Queries.GetPlayerById
{
    /// <summary>
    /// Handles player retrieval by ID
    /// </summary>
    public class GetPlayerByIdQueryHandler : IQueryHandler<GetPlayerByIdQuery, Result<PlayerResponse>>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPlayerByIdQueryHandler> _logger;

        public GetPlayerByIdQueryHandler(
            IPlayerRepository playerRepository,
            IMapper mapper,
            ILogger<GetPlayerByIdQueryHandler> logger)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PlayerResponse>> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Retrieving player: {PlayerId}", request.PlayerId);

                var playerId = PlayerId.Create(request.PlayerId);
                var player = await _playerRepository.GetByPlayerIdAsync(playerId);

                if (player == null)
                {
                    _logger.LogWarning("Player not found: {PlayerId}", request.PlayerId);
                    return Result.Failure<PlayerResponse>($"Player with ID '{request.PlayerId}' not found");
                }

                var response = _mapper.Map<PlayerResponse>(player);
                response.AverageScorePerGame = player.AverageScorePerGame();

                _logger.LogDebug("Successfully retrieved player: {PlayerId}", request.PlayerId);
                return Result.Success(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid player ID format: {PlayerId}", request.PlayerId);
                return Result.Failure<PlayerResponse>("Invalid player ID format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving player: {PlayerId}", request.PlayerId);
                return Result.Failure<PlayerResponse>("An error occurred while retrieving the player");
            }
        }
    }
}