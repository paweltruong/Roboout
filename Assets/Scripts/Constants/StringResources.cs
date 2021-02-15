
public class StringResources
{
    public const string RoboName = "Zero";

    public const string BattleHintText_NoModules = "No modules to use with this output";
    public const string BattleHintText_FoundModules = "Found modules to use with current processor output";
    /// <summary>
    /// {0} name of the robot
    /// </summary>
    public const string BattleHintText_EnemyChoosingModuleFormat = "{0} is considering its options...";
    /// <summary>
    /// {0} name of the robot
    /// </summary>
    public const string BattleHintText_SkipFormat = "{0} is skipping it's turn...";
    
    /// <summary>
    /// source robot name, module name, target name
    /// </summary>
    public const string BattleHintText_EnemyUsingModuleFormat = "{0} is using {1} on {2}...";
    /// <summary>
    /// {0} - amount of damage
    /// {1} - target/victim
    /// {2} - module used
    /// </summary>
    public const string CombatText_Inflicted_KDMG = "Inflicted {0} KDMG to {1} with {2}";
    /// <summary>
    /// {0} - amount of damage
    /// {1} - target/victim
    /// {2} - module used
    /// </summary>
    public const string CombatText_Inflicted_TDMG = "Inflicted {0} TDMG to {1} with {2}";
    /// <summary>
    /// {0} reactor module
    /// {1} amount
    /// </summary>
    public const string CombatText_Heal_Reactor = "{0} reactor regenerated {1} power";
    /// <summary>
    /// {0} module
    /// {1} blocked value
    /// {2} attacker
    /// </summary>
    public const string CombatText_Blocked = "{0} blocked {1} KDMG from {2}";
    /// <summary>
    /// {0} reduced value
    /// {1} attacker
    /// </summary>
    public const string CombatText_Absorbed = "{0} absorbed {1} TDMG from {2}";
    public const string CombatText_Reduced = "Armor reduced {0} TDMG from {1}";
    /// <summary>
    /// {0} modules
    /// {1} outputs
    /// </summary>
    public const string CombatText_FoundModules = "Found modules [{0}] for output [{1}]";
    /// <summary>
    /// {0} outputs
    /// </summary>
    public const string CombatText_NoModules = "No modules for output [{0}]";


    public const string UIText_WhoseTurnFormat = "{0}'s turn";
    public const string UIText_EndTurn = "End Turn";
    /// <summary>
    /// {0} seconds until auto next turn
    /// </summary>
    public const string UIText_EndTurnFormat = "End Turn ({0})";


}

