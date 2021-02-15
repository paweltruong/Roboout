using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RoboMoveStatus
{
    public System.Guid Id;
    public bool Skipped;
    public bool Moved;
    public bool Dead;
    public bool WaitingForMove => !Moved && !Skipped && !Dead;

    internal void Reset()
    {
        this.Skipped = false;
        Moved = false;
    }
}
