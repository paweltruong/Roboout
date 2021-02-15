using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerData : RoboInstanceData
{
    public PlayerData()
        : base()
    {
        this.RoboName = StringResources.RoboName;
        this.Identity = RobotIdentity.Player;
    }
}
