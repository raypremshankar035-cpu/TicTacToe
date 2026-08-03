using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Enums;

namespace TicTacToe.Core.DTOs
{
    public class MoveRequest
    {
        public Player Player { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

}
