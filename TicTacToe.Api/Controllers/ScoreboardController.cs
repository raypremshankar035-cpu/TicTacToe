using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;


namespace TicTacToe.Api.Controllers
{
    [ApiController]
    [Route("api/scoreboard")]
    public class ScoreboardController : ControllerBase
    {
        private readonly IScoreboardService _scoreboardService;

        public ScoreboardController(IScoreboardService scoreboardService)
        {
            _scoreboardService = scoreboardService;
        }

        /// <summary>Get the session-level scoreboard.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(Scoreboard), StatusCodes.Status200OK)]
        public ActionResult<Scoreboard> Get()
        {
            return Ok(_scoreboardService.GetScoreboard());
        }

        /// <summary>Reset X wins / O wins / draws back to zero.</summary>
        [HttpPost("reset")]
        [ProducesResponseType(typeof(Scoreboard), StatusCodes.Status200OK)]
        public ActionResult<Scoreboard> Reset()
        {
            _scoreboardService.ResetScoreboard();
            return Ok(_scoreboardService.GetScoreboard());
        }
    }
}
