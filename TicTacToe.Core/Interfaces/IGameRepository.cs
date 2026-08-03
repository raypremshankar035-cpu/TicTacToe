using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Models;

namespace TicTacToe.Core.Interfaces
{
    public interface IGameRepository
    {
        Game Create(Game game);
        Game? Get(Guid id);
        void Update(Game game);
        void Remove(Guid id);
    }

}
