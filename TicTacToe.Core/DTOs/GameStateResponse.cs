using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Models;


namespace TicTacToe.Core.DTOs
{
    /// <summary>
    /// Everything the Angular frontend needs to render the game in one payload.
    /// </summary>
    public class GameStateResponse
    {
        public Guid GameId { get; set; }
        public string?[][] Board { get; set; } = Array.Empty<string?[]>();
        public Player CurrentPlayer { get; set; }
        public GameMode GameMode { get; set; }
        public GameStatus Status { get; set; }
        public Player? Winner { get; set; }
        public List<WinningCell> WinningCells { get; set; } = new();
        public List<Move> MoveHistory { get; set; } = new();
        public Scoreboard Scoreboard { get; set; } = new();

        /// <summary>
        /// undo eligibility from Status + MoveHistory itself.
        /// </summary>
        public bool CanUndo { get; set; }
    }

}
