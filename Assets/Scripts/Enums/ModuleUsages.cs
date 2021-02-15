using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// determines module usage -> type of effects
/// </summary>
[Flags]
public enum ModuleUsages
{
    None = 0,
    Active = 1,
    /// <summary>
    /// activates in Passive phase if there is enough power and was not pass/skipped
    /// </summary>
    Passive = 2,
    Permanent = 4,
    Active_Passive = 3,
    Active_Permanent = 5,
    Passive_Permanent = 6,
    Active_Passive_Permanent = 7,
}

