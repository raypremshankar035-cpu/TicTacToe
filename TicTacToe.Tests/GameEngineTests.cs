using FluentAssertions;
using TicTacToe.Core.Engine;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Models;
using Xunit;

namespace TicTacToe.Tests
{
    public class GameEngineTests
    {
        private readonly GameEngine _engine = new();

        private static Game NewGame(GameMode mode = GameMode.TwoPlayer) => new() { GameMode = mode };

        [Fact]
        public void IsValidMove_ReturnsTrue_ForEmptyCellAndCorrectTurn()
        {
            var game = NewGame();
            var result = _engine.IsValidMove(game, 0, 0, Player.X, out var error);

            result.Should().BeTrue();
            error.Should().BeNull();
        }

        [Fact]
        public void IsValidMove_ReturnsFalse_ForOccupiedCell()
        {
            var game = NewGame();
            _engine.ApplyMove(game, 0, 0, Player.X);

            var result = _engine.IsValidMove(game, 0, 0, Player.O, out var error);

            result.Should().BeFalse();
            error.Should().Contain("occupied");
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 3)]
        [InlineData(3, 3)]
        public void IsValidMove_ReturnsFalse_ForOutOfRangeCoordinates(int row, int col)
        {
            var game = NewGame();
            var result = _engine.IsValidMove(game, row, col, Player.X, out var error);

            result.Should().BeFalse();
            error.Should().Contain("outside the 3x3 board");
        }

        [Fact]
        public void IsValidMove_ReturnsFalse_ForWrongPlayerTurn()
        {
            var game = NewGame(); // CurrentPlayer defaults to X
            var result = _engine.IsValidMove(game, 0, 0, Player.O, out var error);

            result.Should().BeFalse();
            error.Should().Contain("turn");
        }

        [Fact]
        public void IsValidMove_ReturnsFalse_AfterGameCompleted()
        {
            var game = NewGame();
            game.Status = GameStatus.Won;

            var result = _engine.IsValidMove(game, 0, 0, Player.X, out var error);

            result.Should().BeFalse();
            error.Should().Contain("already completed");
        }

        [Fact]
        public void ApplyMove_SwitchesTurn_AfterValidMove()
        {
            var game = NewGame();
            _engine.ApplyMove(game, 0, 0, Player.X);

            game.CurrentPlayer.Should().Be(Player.O);
        }

        [Fact]
        public void ApplyMove_DoesNotSwitchTurn_WhenMoveEndsTheGame()
        {
            var game = NewGame();
            _engine.ApplyMove(game, 0, 0, Player.X); // X
            _engine.ApplyMove(game, 1, 0, Player.O); // O
            _engine.ApplyMove(game, 0, 1, Player.X); // X
            _engine.ApplyMove(game, 1, 1, Player.O); // O
            _engine.ApplyMove(game, 0, 2, Player.X); // X wins row 0

            game.Status.Should().Be(GameStatus.Won);
            game.Winner.Should().Be(Player.X);
        }

        [Fact]
        public void EvaluateGameState_DetectsRowWin()
        {
            var game = NewGame();
            _engine.ApplyMove(game, 0, 0, Player.X);
            _engine.ApplyMove(game, 1, 0, Player.O);
            _engine.ApplyMove(game, 0, 1, Player.X);
            _engine.ApplyMove(game, 1, 1, Player.O);
            _engine.ApplyMove(game, 0, 2, Player.X);

            game.Status.Should().Be(GameStatus.Won);
            game.Winner.Should().Be(Player.X);
            game.WinningCells.Should().HaveCount(3);
            game.WinningCells.Should().Contain(c => c.Row == 0 && c.Column == 0);
            game.WinningCells.Should().Contain(c => c.Row == 0 && c.Column == 1);
            game.WinningCells.Should().Contain(c => c.Row == 0 && c.Column == 2);
        }

        [Fact]
        public void EvaluateGameState_DetectsColumnWin()
        {
            var game = NewGame();
            _engine.ApplyMove(game, 0, 0, Player.X);
            _engine.ApplyMove(game, 0, 1, Player.O);
            _engine.ApplyMove(game, 1, 0, Player.X);
            _engine.ApplyMove(game, 1, 1, Player.O);
            _engine.ApplyMove(game, 2, 0, Player.X);

            game.Status.Should().Be(GameStatus.Won);
            game.Winner.Should().Be(Player.X);
        }

        [Fact]
        public void EvaluateGameState_DetectsDiagonalWin()
        {
            var game = NewGame();
            _engine.ApplyMove(game, 0, 0, Player.X);
            _engine.ApplyMove(game, 0, 1, Player.O);
            _engine.ApplyMove(game, 1, 1, Player.X);
            _engine.ApplyMove(game, 0, 2, Player.O);
            _engine.ApplyMove(game, 2, 2, Player.X);

            game.Status.Should().Be(GameStatus.Won);
            game.Winner.Should().Be(Player.X);
        }

        [Fact]
        public void EvaluateGameState_DetectsDraw_WhenBoardFullWithNoWinner()
        {
            var game = NewGame();
            // X | O | X
            // X | O | O
            // O | X | X
            _engine.ApplyMove(game, 0, 0, Player.X);
            _engine.ApplyMove(game, 0, 1, Player.O);
            _engine.ApplyMove(game, 0, 2, Player.X);
            _engine.ApplyMove(game, 1, 1, Player.O);
            _engine.ApplyMove(game, 1, 0, Player.X);
            _engine.ApplyMove(game, 1, 2, Player.O);
            _engine.ApplyMove(game, 2, 1, Player.X);
            _engine.ApplyMove(game, 2, 0, Player.O);
            _engine.ApplyMove(game, 2, 2, Player.X);

            game.Status.Should().Be(GameStatus.Draw);
            game.Winner.Should().BeNull();
        }

        [Fact]
        public void ResetBoard_ClearsEverythingButKeepsGameId()
        {
            var game = NewGame();
            var id = game.Id;
            _engine.ApplyMove(game, 0, 0, Player.X);

            _engine.ResetBoard(game);

            game.Id.Should().Be(id);
            game.MoveHistory.Should().BeEmpty();
            game.CurrentPlayer.Should().Be(Player.X);
            game.Status.Should().Be(GameStatus.InProgress);
            game.Board.SelectMany(r => r).Should().OnlyContain(cell => cell == null);
        }

        [Fact]
        public void UndoLastMove_TwoPlayerMode_RemovesOnlyMostRecentMove()
        {
            var game = NewGame(GameMode.TwoPlayer);
            _engine.ApplyMove(game, 0, 0, Player.X);
            _engine.ApplyMove(game, 1, 1, Player.O);

            _engine.UndoLastMove(game);

            game.MoveHistory.Should().HaveCount(1);
            game.Board[1][1].Should().BeNull();
            game.Board[0][0].Should().Be("X");
            game.CurrentPlayer.Should().Be(Player.O);
        }

        [Fact]
        public void UndoLastMove_ComputerMode_RemovesComputerMoveAndPriorHumanMove()
        {
            var game = NewGame(GameMode.Computer);
            _engine.ApplyMove(game, 0, 0, Player.X); // human
            _engine.ApplyMove(game, 1, 1, Player.O); // computer

            _engine.UndoLastMove(game);

            game.MoveHistory.Should().BeEmpty();
            game.Board.SelectMany(r => r).Should().OnlyContain(cell => cell == null);
            game.CurrentPlayer.Should().Be(Player.X);
        }

        [Fact]
        public void UndoLastMove_DoesNothing_WhenNoMovesExist()
        {
            var game = NewGame();
            _engine.UndoLastMove(game);

            game.MoveHistory.Should().BeEmpty();
            game.CurrentPlayer.Should().Be(Player.X);
        }

        [Fact]
        public void GetComputerMove_TakesWinningMove_WhenAvailable()
        {
            var game = NewGame(GameMode.Computer);
            // O has two in a row (0,0) and (0,1); (0,2) completes the win.
            game.Board[0][0] = "O";
            game.Board[0][1] = "O";
            game.Board[1][0] = "X";
            game.Board[1][1] = "X";

            var move = _engine.GetComputerMove(game);

            move.Should().Be((0, 2));
        }

        [Fact]
        public void GetComputerMove_BlocksOpponent_WhenNoWinAvailable()
        {
            var game = NewGame(GameMode.Computer);
            // X has two in a row (1,0) and (1,1); O must block at (1,2).
            game.Board[1][0] = "X";
            game.Board[1][1] = "X";
            game.Board[0][0] = "O";

            var move = _engine.GetComputerMove(game);

            move.Should().Be((1, 2));
        }

        [Fact]
        public void GetComputerMove_TakesCenter_WhenAvailableAndNoWinOrBlock()
        {
            var game = NewGame(GameMode.Computer);
            game.Board[0][0] = "X";

            var move = _engine.GetComputerMove(game);

            move.Should().Be((1, 1));
        }

        [Fact]
        public void GetComputerMove_TakesCorner_WhenCenterTaken()
        {
            var game = NewGame(GameMode.Computer);
            game.Board[1][1] = "X";

            var move = _engine.GetComputerMove(game);

            move.Should().BeOneOf((0, 0), (0, 2), (2, 0), (2, 2));
        }
    }
}