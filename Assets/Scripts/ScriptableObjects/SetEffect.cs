using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data about bonuses if robo wears specific module set
/// </summary>
[CreateAssetMenu(fileName ="SETEFF000", menuName ="Set Effect")]
[System.Serializable]
public class SetEffect : ScriptableObject
{
    public string SetName;
    public string Description;
    /// <summary>
    /// What modules are considered full set
    /// </summary>
    public Module[] SetParts;
    
    [Header("Passive Effects")]
    public int BonusMaxHp;
    public int BonusShield;
    [Range(0,1)]
    public float BonusScavenging;
    /// <summary>
    /// bolt drop multiplier 1 = 100%, (additional)
    /// </summary>
    [Range(0,10)]
    public float BonusSalvaging;
    public int BonusHpRegen;
    public int BonusDamageThermal;
    public int BonusDamageKinetic;
    /// <summary>
    /// display additional info about available encounters
    /// </summary>
    public bool RevealEncounterInfo;
    public int BonusArmor;
    public int RegenHpAfterKill;
    /// <summary>
    /// ensures the overclock value overrides one of rolled dice value, so the overclock value will always be rolled on dice(processor output)
    /// </summary>
    [Range(0,6)]
    public int ProcessorOverclock;
}
