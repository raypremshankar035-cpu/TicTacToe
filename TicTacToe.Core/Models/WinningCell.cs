using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.Models
{
    public class WinningCell
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public WinningCell() { }

        public WinningCell(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }

}
