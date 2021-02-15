using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum EncounterType
{
    None = 0,
    Tutorial=1,
    Danger=2,//battle
    /// <summary>
    ///  buy, sell , heal for gold/bolts
    /// </summary>
    Blackmarket=3,
    /// <summary>
    /// uninstall/replace/upgrade modules
    /// </summary>
    Workshop=4,
    
    /// <summary>
    /// heal for free
    /// </summary>
    PowerStation=5,
    /// <summary>
    /// replace robot body
    /// </summary>
    MainframeFitting=6,
    Boss=7,//battle
    Goal=8,
}
