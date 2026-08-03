using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Models;


namespace TicTacToe.Core.Interfaces
{
    
    public interface IGameEngine
    {
        bool IsValidMove(Game game, int row, int column, Player player, out string? error);
        void ApplyMove(Game game, int row, int column, Player player);
        void EvaluateGameState(Game game);
        void UndoLastMove(Game game);
        void ResetBoard(Game game);
        (int Row, int Column)? GetComputerMove(Game game);
    }

}
