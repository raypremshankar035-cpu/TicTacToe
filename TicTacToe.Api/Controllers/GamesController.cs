using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Core.DTOs;
using TicTacToe.Core.Interfaces;


namespace TicTacToe.Api.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>Create a new game session.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status201Created)]
        public ActionResult<GameStateResponse> CreateGame([FromBody] CreateGameRequest request)
        {
            var result = _gameService.CreateGame(request);
            return CreatedAtAction(nameof(GetGame), new { id = result.GameId }, result);
        }

        /// <summary>Get the current state of a game.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GameStateResponse> GetGame(Guid id)
        {
            return Ok(_gameService.GetGame(id));
        }

        /// <summary>Submit a move for a player.</summary>
        [HttpPost("{id:guid}/moves")]
        [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GameStateResponse> MakeMove(Guid id, [FromBody] MoveRequest request)
        {
            return Ok(_gameService.MakeMove(id, request));
        }

        /// <summary>Undo the last move (behavior depends on game mode — see README).</summary>
        [HttpPost("{id:guid}/undo")]
        [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GameStateResponse> Undo(Guid id)
        {
            return Ok(_gameService.Undo(id));
        }

        /// <summary>Reset the current game (keeps the scoreboard unchanged).</summary>
        [HttpPost("{id:guid}/reset")]
        [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GameStateResponse> Reset(Guid id)
        {
            return Ok(_gameService.Reset(id));
        }
    }
}
