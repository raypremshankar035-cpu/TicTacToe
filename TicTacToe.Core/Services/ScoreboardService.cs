using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;


namespace TicTacToe.Core.Services
{
    public class ScoreboardService : IScoreboardService
    {
        private readonly object _lock = new();
        private Scoreboard _scoreboard = new();

        public Scoreboard GetScoreboard()
        {
            lock (_lock)
            {
                return new Scoreboard
                {
                    XWins = _scoreboard.XWins,
                    OWins = _scoreboard.OWins,
                    Draws = _scoreboard.Draws
                };
            }
        }

        public void RecordResult(GameStatus status, Player? winner)
        {
            lock (_lock)
            {
                if (status == GameStatus.Won && winner == Player.X) _scoreboard.XWins++;
                else if (status == GameStatus.Won && winner == Player.O) _scoreboard.OWins++;
                else if (status == GameStatus.Draw) _scoreboard.Draws++;
            }
        }

        public void UndoResult(GameStatus status, Player? winner)
        {
           
            lock (_lock)
            {
                if (status == GameStatus.Won && winner == Player.X && _scoreboard.XWins > 0) _scoreboard.XWins--;
                else if (status == GameStatus.Won && winner == Player.O && _scoreboard.OWins > 0) _scoreboard.OWins--;
                else if (status == GameStatus.Draw && _scoreboard.Draws > 0) _scoreboard.Draws--;
            }
        }

        public void ResetScoreboard()
        {
            lock (_lock)
            {
                _scoreboard = new Scoreboard();
            }
        }
    }
}
