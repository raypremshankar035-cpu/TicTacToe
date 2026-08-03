using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Enums;

namespace TicTacToe.Core.Models
{
 
    public class Game
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string?[][] Board { get; set; } = new string?[3][]
        {
            new string?[3],
            new string?[3],
            new string?[3]
        };

        public Player CurrentPlayer { get; set; } = Player.X;

        public GameMode GameMode { get; set; }

        public GameStatus Status { get; set; } = GameStatus.InProgress;

        public Player? Winner { get; set; }

        public List<WinningCell> WinningCells { get; set; } = new();

        public List<Move> MoveHistory { get; set; } = new();

        public bool ScoreRecorded { get; set; } = false;
    }

}
