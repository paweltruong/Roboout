using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Effect
{
    //values in enum typed manually so ScriptableObjects and prefabs are stable after adding/removing values
    None = 0,
    PowerCost=1,
    DamageThermal=2,
    DamageKinetic=3,
    DisableModuleForXTurns=4,    //dual
    PowerSubstractEachTurnOverXTurns=5,//dual
    Block=6,
    BonusMaxPower=7,
    ThermalShieldForXTurns=8,//dual
    PowerRegen=9,
    BonusScavenging=10,//perc
    BonusSalvaging=11,//perc
    BonusDamageThermal=12,
    BonusDamageKinetic=13,
    Scout=14,
    RegenPowerForKill=15,
    StealPower=16,
    Armor=17,
    Overclock=18 //18
}
