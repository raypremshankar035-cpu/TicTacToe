using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.DTOs;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;


namespace TicTacToe.Core.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _repository;
        private readonly IGameEngine _engine;
        private readonly IScoreboardService _scoreboardService;

        public GameService(IGameRepository repository, IGameEngine engine, IScoreboardService scoreboardService)
        {
            _repository = repository;
            _engine = engine;
            _scoreboardService = scoreboardService;
        }

        public GameStateResponse CreateGame(CreateGameRequest request)
        {
            var game = new Game { GameMode = request.GameMode };
            _repository.Create(game);
            return MapToResponse(game);
        }

        public GameStateResponse GetGame(Guid gameId)
        {
            var game = GetGameOrThrow(gameId);
            return MapToResponse(game);
        }

        public GameStateResponse MakeMove(Guid gameId, MoveRequest request)
        {
            var game = GetGameOrThrow(gameId);

            if (!_engine.IsValidMove(game, request.Row, request.Column, request.Player, out var error))
            {
                throw new InvalidMoveException(error!);
            }

            _engine.ApplyMove(game, request.Row, request.Column, request.Player);
            RecordScoreIfCompleted(game);

            
            if (game.GameMode == GameMode.Computer &&
                game.Status == GameStatus.InProgress &&
                game.CurrentPlayer == Player.O)
            {
                var computerMove = _engine.GetComputerMove(game);
                if (computerMove != null)
                {
                    _engine.ApplyMove(game, computerMove.Value.Row, computerMove.Value.Column, Player.O);
                    RecordScoreIfCompleted(game);
                }
            }

            _repository.Update(game);
            return MapToResponse(game);
        }

        public GameStateResponse Undo(Guid gameId)
        {
            var game = GetGameOrThrow(gameId);

            if (game.MoveHistory.Count == 0)
            {
                throw new InvalidMoveException("Undo rejected: there are no moves to undo.");
            }

      
            if (game.Status != GameStatus.InProgress)
            {
                throw new InvalidMoveException("Undo rejected: the game has already completed. Reset or start a new game instead.");
            }

            _engine.UndoLastMove(game);
            _repository.Update(game);
            return MapToResponse(game);
        }

        public GameStateResponse Reset(Guid gameId)
        {
            var game = GetGameOrThrow(gameId);
            _engine.ResetBoard(game);
            _repository.Update(game);
            return MapToResponse(game);
        }

        private void RecordScoreIfCompleted(Game game)
        {
            if (game.Status != GameStatus.InProgress && !game.ScoreRecorded)
            {
                _scoreboardService.RecordResult(game.Status, game.Winner);
                game.ScoreRecorded = true;
            }
        }

        private Game GetGameOrThrow(Guid gameId)
        {
            return _repository.Get(gameId) ?? throw new GameNotFoundException(gameId);
        }

        private GameStateResponse MapToResponse(Game game)
        {
            return new GameStateResponse
            {
                GameId = game.Id,
                Board = game.Board,
                CurrentPlayer = game.CurrentPlayer,
                GameMode = game.GameMode,
                Status = game.Status,
                Winner = game.Winner,
                WinningCells = game.WinningCells,
                MoveHistory = game.MoveHistory,
                Scoreboard = _scoreboardService.GetScoreboard(),
                CanUndo = game.Status == GameStatus.InProgress && game.MoveHistory.Count > 0
            };
        }
    }
}
