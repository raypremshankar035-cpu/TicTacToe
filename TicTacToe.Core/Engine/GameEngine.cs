using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;


namespace TicTacToe.Core.Engine
{
    public class GameEngine : IGameEngine
    {
        // The 8 winning lines on a 3x3 board: 3 rows, 3 columns, 2 diagonals.
        private static readonly (int Row, int Col)[][] WinningLines = new (int, int)[][]
        {
            new[] { (0, 0), (0, 1), (0, 2) }, // row 0
            new[] { (1, 0), (1, 1), (1, 2) }, // row 1
            new[] { (2, 0), (2, 1), (2, 2) }, // row 2
            new[] { (0, 0), (1, 0), (2, 0) }, // col 0
            new[] { (0, 1), (1, 1), (2, 1) }, // col 1
            new[] { (0, 2), (1, 2), (2, 2) }, // col 2
            new[] { (0, 0), (1, 1), (2, 2) }, // diagonal \
            new[] { (0, 2), (1, 1), (2, 0) }, // diagonal /
        };

        public bool IsValidMove(Game game, int row, int column, Player player, out string? error)
        {
            error = null;

            if (game.Status != GameStatus.InProgress)
            {
                error = "Move rejected: the game has already completed.";
                return false;
            }

            if (row < 0 || row > 2 || column < 0 || column > 2)
            {
                error = "Move rejected: cell coordinates are outside the 3x3 board.";
                return false;
            }

            if (game.Board[row][column] != null)
            {
                error = "Move rejected: the selected cell is already occupied.";
                return false;
            }

            if (game.CurrentPlayer != player)
            {
                error = $"Move rejected: it is currently {game.CurrentPlayer}'s turn.";
                return false;
            }

            return true;
        }
        public void ApplyMove(Game game, int row, int column, Player player)
        {
            game.Board[row][column] = player.ToString();

            var moveNumber = game.MoveHistory.Count + 1;
            game.MoveHistory.Add(new Move
            {
                MoveNumber = moveNumber,
                Player = player,
                Row = row,
                Column = column
            });

            EvaluateGameState(game);

            // Only advance the turn if the game is still in progress.
            if (game.Status == GameStatus.InProgress)
            {
                game.CurrentPlayer = player == Player.X ? Player.O : Player.X;
            }
        }

        public void EvaluateGameState(Game game)
        {
            foreach (var line in WinningLines)
            {
                var a = game.Board[line[0].Row][line[0].Col];
                var b = game.Board[line[1].Row][line[1].Col];
                var c = game.Board[line[2].Row][line[2].Col];

                if (a != null && a == b && b == c)
                {
                    game.Status = GameStatus.Won;
                    game.Winner = Enum.Parse<Player>(a);
                    game.WinningCells = new List<WinningCell>
                    {
                        new(line[0].Row, line[0].Col),
                        new(line[1].Row, line[1].Col),
                        new(line[2].Row, line[2].Col),
                    };
                    return;
                }
            }

            var isFull = game.Board.SelectMany(r => r).All(cell => cell != null);
            if (isFull)
            {
                game.Status = GameStatus.Draw;
                game.Winner = null;
                game.WinningCells.Clear();
            }
        }

        public void UndoLastMove(Game game)
        {
            if (game.MoveHistory.Count == 0)
            {
                return;
            }

            // Two Player Mode: remove only the most recent move.
            // Computer Mode: remove the computer's last move AND the human move before it,
            // so it becomes the human's turn again.
            var movesToRemove = game.GameMode == GameMode.Computer ? 2 : 1;
            movesToRemove = Math.Min(movesToRemove, game.MoveHistory.Count);

            for (var i = 0; i < movesToRemove; i++)
            {
                var last = game.MoveHistory[^1];
                game.Board[last.Row][last.Column] = null;
                game.MoveHistory.RemoveAt(game.MoveHistory.Count - 1);
            }

            // Recompute status/winner/winning cells from the restored board
            game.Status = GameStatus.InProgress;
            game.Winner = null;
            game.WinningCells.Clear();
            game.ScoreRecorded = false;

            EvaluateGameState(game);

            if (game.Status == GameStatus.InProgress)
            {
                game.CurrentPlayer = game.MoveHistory.Count == 0
                    ? Player.X
                    : (game.MoveHistory[^1].Player == Player.X ? Player.O : Player.X);
            }
        }

        public void ResetBoard(Game game)
        {
            game.Board = new string?[3][]
            {
                new string?[3],
                new string?[3],
                new string?[3]
            };
            game.MoveHistory.Clear();
            game.CurrentPlayer = Player.X;
            game.Status = GameStatus.InProgress;
            game.Winner = null;
            game.WinningCells.Clear();
            game.ScoreRecorded = false;
        }

        public (int Row, int Column)? GetComputerMove(Game game)
        {
            var board = game.Board;

            // Priority 1: if O can win this turn, take the winning move.
            var winMove = FindTwoInARowWithEmpty(board, Player.O);
            if (winMove != null)
            {
                return winMove;
            }

            // Priority 2: if X could win next turn, block it.
            var blockMove = FindTwoInARowWithEmpty(board, Player.X);
            if (blockMove != null)
            {
                return blockMove;
            }

            // Priority 3: take the center.
            if (board[1][1] == null)
            {
                return (1, 1);
            }

            // Priority 4: take the first available corner.
            var corners = new (int Row, int Col)[] { (0, 0), (0, 2), (2, 0), (2, 2) };
            foreach (var (row, col) in corners)
            {
                if (board[row][col] == null)
                {
                    return (row, col);
                }
            }

            // Priority 5: take any available cell.
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (board[row][col] == null)
                    {
                        return (row, col);
                    }
                }
            }

            return null; // board is full 
        }

        private static (int Row, int Column)? FindTwoInARowWithEmpty(string?[][] board, Player player)
        {
            var symbol = player.ToString();

            foreach (var line in WinningLines)
            {
                var cells = new[]
                {
                    board[line[0].Row][line[0].Col],
                    board[line[1].Row][line[1].Col],
                    board[line[2].Row][line[2].Col],
                };

                var symbolCount = cells.Count(v => v == symbol);
                var emptyIndex = Array.IndexOf(cells, null);

                if (symbolCount == 2 && emptyIndex != -1)
                {
                    return line[emptyIndex];
                }
            }

            return null;
        }
    }

}
