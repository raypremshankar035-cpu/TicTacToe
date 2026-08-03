using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.Exceptions
{
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(Guid gameId)
            : base($"Game '{gameId}' was not found.")
        {
        }
    }

}
