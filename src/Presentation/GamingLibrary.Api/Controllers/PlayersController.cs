// src/Presentation/GamingLibrary.Api/Controllers/PlayersController.cs
// Purpose: REST API controller for player operations using CQRS
using GamingLibrary.Application.Features.Players.Commands.CreatePlayer;
using GamingLibrary.Application.Features.Players.Queries.GetPlayerById;
using GamingLibrary.Application.Features.Players.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GamingLibrary.Api.Controllers
{
    /// <summary>
    /// REST API controller for player management operations.
    /// Demonstrates industry-standard API design with CQRS pattern.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class PlayersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(IMediator mediator, ILogger<PlayersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new player account
        /// </summary>
        /// <param name="request">Player creation data</param>
        /// <returns>Created player information</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PlayerResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PlayerResponse>> CreatePlayer([FromBody] CreatePlayerRequest request)
        {
            _logger.LogInformation("Creating player with email: {Email}", request.Email);

            var command = new CreatePlayerCommand(request.Name, request.Email);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to create player: {Error}", result.Error);
                
                // Return appropriate HTTP status based on error type
                if (result.Error.Contains("already exists"))
                    return Conflict(result.Error);
                
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully created player: {PlayerId}", result.Value.PlayerId);
            
            return CreatedAtAction(
                nameof(GetPlayerById), 
                new { playerId = result.Value.PlayerId }, 
                result.Value);
        }

        /// <summary>
        /// Retrieves a player by their unique ID
        /// </summary>
        /// <param name="playerId">Player unique identifier</param>
        /// <returns>Player information</returns>
        [HttpGet("{playerId}")]
        [ProducesResponseType(typeof(PlayerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlayerResponse>> GetPlayerById([FromRoute] string playerId)
        {
            _logger.LogDebug("Getting player: {PlayerId}", playerId);

            var query = new GetPlayerByIdQuery(playerId);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get player {PlayerId}: {Error}", playerId, result.Error);
                
                if (result.Error.Contains("not found"))
                    return NotFound(result.Error);
                
                return BadRequest(result.Error);
            }

            _logger.LogDebug("Successfully retrieved player: {PlayerId}", playerId);
            return Ok(result.Value);
        }

        /// <summary>
        /// Gets the top players by score for leaderboards
        /// </summary>
        /// <param name="count">Number of top players to return (default: 10, max: 100)</param>
        /// <returns>List of top players</returns>
        [HttpGet("leaderboard")]
        [ProducesResponseType(typeof(IEnumerable<PlayerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PlayerResponse>>> GetLeaderboard([FromQuery] int count = 10)
        {
            if (count <= 0 || count > 100)
                return BadRequest("Count must be between 1 and 100");

            _logger.LogDebug("Getting leaderboard with {Count} players", count);

            // TODO: Implement GetTopPlayersQuery
            // var query = new GetTopPlayersQuery(count);
            // var result = await _mediator.Send(query);

            // Placeholder response
            return Ok(new List<PlayerResponse>());
        }
    }

    /// <summary>
    /// Request model for creating a new player
    /// </summary>
    public class CreatePlayerRequest
    {
        /// <summary>Player display name</summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>Player email address</summary>
        public string Email { get; set; } = string.Empty;
    }
}