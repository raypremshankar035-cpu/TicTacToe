using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Models;


namespace TicTacToe.Core.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly ConcurrentDictionary<Guid, Game> _games = new();

        public Game Create(Game game)
        {
            _games[game.Id] = game;
            return game;
        }

        public Game? Get(Guid id)
        {
            return _games.TryGetValue(id, out var game) ? game : null;
        }

        public void Update(Game game)
        {
            _games[game.Id] = game;
        }

        public void Remove(Guid id)
        {
            _games.TryRemove(id, out _);
        }
    }

}
