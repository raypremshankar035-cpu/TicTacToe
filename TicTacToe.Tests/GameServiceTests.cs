using FluentAssertions;
using TicTacToe.Core.DTOs;
using TicTacToe.Core.Engine;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions;
using TicTacToe.Core.Repositories;
using TicTacToe.Core.Services;
using Xunit;


namespace TicTacToe.Tests
{
    public class GameServiceTests
    {
        private static GameService BuildService(out ScoreboardService scoreboardService)
        {
            scoreboardService = new ScoreboardService();
            return new GameService(new GameRepository(), new GameEngine(), scoreboardService);
        }

        [Fact]
        public void CreateGame_ReturnsInProgressGame_WithEmptyBoard()
        {
            var service = BuildService(out _);

            var response = service.CreateGame(new CreateGameRequest { GameMode = GameMode.TwoPlayer });

            response.Status.Should().Be(GameStatus.InProgress);
            response.CurrentPlayer.Should().Be(Player.X);
            response.Board.SelectMany(r => r).Should().OnlyContain(cell => cell == null);
        }

        [Fact]
        public void MakeMove_RejectsMoveByWrongPlayer()
        {
            var service = BuildService(out _);
            var game = service.CreateGame(new CreateGameRequest { GameMode = GameMode.TwoPlayer });

            var act = () => service.MakeMove(game.GameId, new MoveRequest { Player = Player.O, Row = 0, Column = 0 });

            act.Should().Throw<InvalidMoveException>();
        }

        [Fact]
        public void MakeMove_ThrowsGameNotFoundException_ForUnknownGameId()
        {
            var service = BuildService(out _);

            var act = () => service.MakeMove(Guid.NewGuid(), new MoveRequest { Player = Player.X, Row = 0, Column = 0 });

            act.Should().Throw<GameNotFoundException>();
        }

        [Fact]
        public void MakeMove_UpdatesScoreboardExactlyOnce_WhenGameIsWon()
        {
            var service = BuildService(out var scoreboardService);
            var game = service.CreateGame(new CreateGameRequest { GameMode = GameMode.TwoPlayer });

            service.MakeMove(game.GameId, new MoveRequest { Player = Player.X, Row = 0, Column = 0 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.O, Row = 1, Column = 0 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.X, Row = 0, Column = 1 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.O, Row = 1, Column = 1 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.X, Row = 0, Column = 2 }); // X wins

            scoreboardService.GetScoreboard().XWins.Should().Be(1);

         
            service.GetGame(game.GameId);
            scoreboardService.GetScoreboard().XWins.Should().Be(1);
        }

        [Fact]
        public void MakeMove_ComputerMode_AutoPlaysO_AfterValidHumanMove()
        {
            var service = BuildService(out _);
            var game = service.CreateGame(new CreateGameRequest { GameMode = GameMode.Computer });

            var response = service.MakeMove(game.GameId, new MoveRequest { Player = Player.X, Row = 0, Column = 0 });

            response.MoveHistory.Should().HaveCount(2); // human move + computer move
            response.MoveHistory[1].Player.Should().Be(Player.O);
            response.CurrentPlayer.Should().Be(Player.X);
        }

        [Fact]
        public void Undo_ThrowsInvalidMoveException_WhenGameAlreadyCompleted()
        {
            var service = BuildService(out _);
            var game = service.CreateGame(new CreateGameRequest { GameMode = GameMode.TwoPlayer });

            service.MakeMove(game.GameId, new MoveRequest { Player = Player.X, Row = 0, Column = 0 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.O, Row = 1, Column = 0 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.X, Row = 0, Column = 1 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.O, Row = 1, Column = 1 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.X, Row = 0, Column = 2 }); // X wins

            var act = () => service.Undo(game.GameId);

            act.Should().Throw<InvalidMoveException>().WithMessage("*already completed*");
        }

        [Fact]
        public void Reset_ClearsBoardAndHistory_ButKeepsScoreboardUnchanged()
        {
            var service = BuildService(out var scoreboardService);
            var game = service.CreateGame(new CreateGameRequest { GameMode = GameMode.TwoPlayer });

            service.MakeMove(game.GameId, new MoveRequest { Player = Player.X, Row = 0, Column = 0 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.O, Row = 1, Column = 0 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.X, Row = 0, Column = 1 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.O, Row = 1, Column = 1 });
            service.MakeMove(game.GameId, new MoveRequest { Player = Player.X, Row = 0, Column = 2 }); // X wins

            var scoreBefore = scoreboardService.GetScoreboard().XWins;

            var response = service.Reset(game.GameId);

            response.Status.Should().Be(GameStatus.InProgress);
            response.MoveHistory.Should().BeEmpty();
            scoreboardService.GetScoreboard().XWins.Should().Be(scoreBefore);
        }
    }
}
