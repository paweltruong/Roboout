using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// module configuration
/// </summary>
[CreateAssetMenu(fileName ="MD000", menuName ="Robo Module")]
[System.Serializable]
public class Module : ScriptableObject
{
    public string ModuleName;
    public ModuleKey Key;
    public string Desc;
    public SlotType Slot;
    public int Rank;
    /// <summary>
    /// sell to vendor
    /// </summary>
    public int SellValue = 5;
    public int BuyVallue = 10;
    public int MaxBoltsDrop = 10;
    public Module NextRank;
    public int PowerCost;
    public int UpgradeCost;
    public ModuleQuality Quality = ModuleQuality.Common;
    public ModuleUsages Usage;
    public AnimationType Animation;
    public AllowedTargets[] AllowedTargets;
    [Header("Active Effects")]
    public DiceRoll DiceToActivate;
    public int DamageThermal;
    public int DamageKinetic;
    public int DamageKineticCleave;
    public int DisableRandomForXTurns;
    public int ReduceXPower;
    public int ReducePowerForXTurns;
    public int BlockStrength;
    /// <summary>
    /// steal power
    /// </summary>
    public int HpForEnemy;
    [Header("Passive Effects")]
    public int BonusMaxHp;
    public int BonusShield;
    /// <summary>
    /// chance of module dropping
    /// </summary>
    [Range(0,1)]
    public float BonusScavenging;
    /// <summary>
    /// bonus% of gold/bolts
    /// </summary>
    [Range(0,10)]
    public float BonusSalvaging;
    public int BonusHpRegen;
    public int BonusDamageThermal;
    public int BonusDamageKinetic;
    public bool RevealHpAndModules;
    public bool RevealDungeonInfo;
    public int BonusArmor;
    public int RegenPowerAfterKill;
    /// <summary>
    /// ensures the specific value is on the dice / cpu output
    /// </summary>
    [Range(0,6)]
    public int ProcessorOverclock;




}
