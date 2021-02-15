using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum EncounterRandomConfig
{
    /// <summary>
    /// YES:danger, boss, shop or pickup NO:goal, tutorial
    /// </summary>
    Default = 0,
    /// <summary>
    /// YES:shop or pickup NO:danger,boss,goal, tutorial
    /// </summary>
    OtherThanDangerButNotGoal = 1,
    /// <summary>
    /// YES:shop or pickup NO:danger,boss, tutorial
    /// </summary>
    OtherThanDangerPossibleGoal = 2,
    /// <summary>
    /// YES:danger NO:boss,goal, tutorial, shop or pickup
    /// </summary>
    Danger = 3,
    /// <summary>
    /// YES:boss NO:danger,goal, tutorial, shop or pickup
    /// </summary>
    Boss = 4,
    Tutorial = 5,
    Goal = 6,
    BossOrDanger = 7,
    GoalOrMainframeFitting =8,
    MainframeFitting = 9,
}
