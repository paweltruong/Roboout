using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Flags]
public enum DiceRoll
{
    None = 0,
    D1 = (1 << 1),
    D2 = (1 << 2),
    D3 = (1 << 3),
    D4 = (1 << 4),
    D5 = (1 << 5),
    D6 = (1 << 6),
}

