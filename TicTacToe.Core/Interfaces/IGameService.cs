using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.DTOs;

namespace TicTacToe.Core.Interfaces
{
    public interface IGameService
    {
        GameStateResponse CreateGame(CreateGameRequest request);
        GameStateResponse GetGame(Guid gameId);
        GameStateResponse MakeMove(Guid gameId, MoveRequest request);
        GameStateResponse Undo(Guid gameId);
        GameStateResponse Reset(Guid gameId);
    }

}
