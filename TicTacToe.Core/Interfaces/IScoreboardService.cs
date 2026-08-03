using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Models;


namespace TicTacToe.Core.Interfaces
{
    public interface IScoreboardService
    {
        Scoreboard GetScoreboard();
        void RecordResult(GameStatus status, Player? winner);
        void UndoResult(GameStatus status, Player? winner);
        void ResetScoreboard();
    }

}
