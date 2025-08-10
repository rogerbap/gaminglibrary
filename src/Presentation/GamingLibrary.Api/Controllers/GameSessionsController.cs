// src/Presentation/GamingLibrary.Api/Controllers/GameSessionsController.cs
// Purpose: REST API controller for game session operations
using GamingLibrary.Application.Features.GameSessions.Commands.StartGameSession;
using GamingLibrary.Application.Features.GameSessions.Commands.EndGameSession;
using GamingLibrary.Application.Features.GameSessions.Queries.GetSessionById;
using GamingLibrary.Application.Features.GameSessions.DTOs;
using GamingLibrary.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GamingLibrary.Api.Controllers
{
    /// <summary>
    /// REST API controller for game session management.
    /// Handles session lifecycle: start, update, end, and retrieval.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class GameSessionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GameSessionsController> _logger;

        public GameSessionsController(IMediator mediator, ILogger<GameSessionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Starts a new game session for a player
        /// </summary>
        /// <param name="request">Session start data</param>
        /// <returns>Created session information</returns>
        [HttpPost("start")]
        [ProducesResponseType(typeof(GameSessionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GameSessionResponse>> StartSession([FromBody] StartSessionRequest request)
        {
            _logger.LogInformation("Starting {GameType} session for player: {PlayerId}", 
                request.GameType, request.PlayerId);

            var command = new StartGameSessionCommand(request.PlayerId, request.GameType);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to start session: {Error}", result.Error);
                
                if (result.Error.Contains("not found"))
                    return NotFound(result.Error);
                
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully started session: {SessionId}", result.Value.SessionId);
            
            return CreatedAtAction(
                nameof(GetSessionById), 
                new { sessionId = result.Value.SessionId }, 
                result.Value);
        }

        /// <summary>
        /// Ends an active game session
        /// </summary>
        /// <param name="sessionId">Session to end</param>
        /// <param name="request">Session end data</param>
        /// <returns>Completed session information</returns>
        [HttpPost("{sessionId}/end")]
        [ProducesResponseType(typeof(GameSessionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GameSessionResponse>> EndSession(
            [FromRoute] string sessionId, 
            [FromBody] EndSessionRequest request)
        {
            _logger.LogInformation("Ending session: {SessionId} with score: {Score}", 
                sessionId, request.FinalScore);

            var command = new EndGameSessionCommand(
                sessionId, 
                request.FinalScore, 
                request.CompletedSuccessfully,
                request.FinalGameData);
                
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to end session {SessionId}: {Error}", sessionId, result.Error);
                
                if (result.Error.Contains("not found"))
                    return NotFound(result.Error);
                
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Successfully ended session: {SessionId}", sessionId);
            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieves a game session by ID
        /// </summary>
        /// <param name="sessionId">Session unique identifier</param>
        /// <returns>Session information</returns>
        [HttpGet("{sessionId}")]
        [ProducesResponseType(typeof(GameSessionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GameSessionResponse>> GetSessionById([FromRoute] string sessionId)
        {
            _logger.LogDebug("Getting session: {SessionId}", sessionId);

            var query = new GetSessionByIdQuery(sessionId);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get session {SessionId}: {Error}", sessionId, result.Error);
                
                if (result.Error.Contains("not found"))
                    return NotFound(result.Error);
                
                return BadRequest(result.Error);
            }

            _logger.LogDebug("Successfully retrieved session: {SessionId}", sessionId);
            return Ok(result.Value);
        }

        /// <summary>
        /// Gets all sessions for a specific player
        /// </summary>
        /// <param name="playerId">Player ID</param>
        /// <param name="gameType">Optional filter by game type</param>
        /// <param name="limit">Maximum number of sessions to return</param>
        /// <returns>List of player's sessions</returns>
        [HttpGet("player/{playerId}")]
        [ProducesResponseType(typeof(IEnumerable<GameSessionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<GameSessionResponse>>> GetPlayerSessions(
            [FromRoute] string playerId,
            [FromQuery] GameType? gameType = null,
            [FromQuery] int limit = 50)
        {
            if (limit <= 0 || limit > 100)
                return BadRequest("Limit must be between 1 and 100");

            _logger.LogDebug("Getting sessions for player: {PlayerId}", playerId);

            // TODO: Implement GetPlayerSessionsQuery handler
            // var query = new GetPlayerSessionsQuery(playerId, gameType, limit);
            // var result = await _mediator.Send(query);

            // Placeholder response
            return Ok(new List<GameSessionResponse>());
        }
    }

    /// <summary>
    /// Request model for starting a game session
    /// </summary>
    public class StartSessionRequest
    {
        /// <summary>Player ID starting the session</summary>
        public string PlayerId { get; set; } = string.Empty;
        
        /// <summary>Type of game to play</summary>
        public GameType GameType { get; set; }
    }

    /// <summary>
    /// Request model for ending a game session
    /// </summary>
    public class EndSessionRequest
    {
        /// <summary>Final score achieved in the session</summary>
        public int FinalScore { get; set; }
        
        /// <summary>Whether the session was completed successfully</summary>
        public bool CompletedSuccessfully { get; set; }
        
        /// <summary>Optional final game-specific data</summary>
        public Dictionary<string, object>? FinalGameData { get; set; }
    }
}
