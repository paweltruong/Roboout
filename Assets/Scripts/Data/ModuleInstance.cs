using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleInstance
{
    public System.Guid Id;
    public int SlotIndex;
    /// <summary>
    /// module has not beed disrupted and is ready to use
    /// </summary>
    public bool IsEnabled;

    public bool IsActiveEffectModule => Usage.HasFlag(ModuleUsages.Active);

    public AllowedTargets[] AllowedTargets;

    public ModuleInstance()
        : base()
    {
    }

    public ModuleInstance(Module m, int assignSlotIndex)
        : this()
    {
        this.SlotIndex = assignSlotIndex;
        this.Id = System.Guid.NewGuid();
        this.ModuleName = m.ModuleName;
        this.Key = m.Key;
        this.Desc = m.Desc;
        this.Slot = m.Slot;
        this.Rank = m.Rank;
        this.SellValue = m.SellValue;
        this.MaxBoltsDrop = m.MaxBoltsDrop;
        this.NextRank = m.NextRank;
        this.PowerCost = m.PowerCost;
        this.UpgradeCost = m.UpgradeCost;
        this.Quality = m.Quality;
        this.Usage = m.Usage;
        this.Animation = m.Animation;
        this.AllowedTargets = m.AllowedTargets;

        this.DiceToActivate = m.DiceToActivate;
        this.DamageThermal = m.DamageThermal;
        this.DamageKinetic = m.DamageKinetic;
        this.DamageKineticCleave = m.DamageKineticCleave;
        this.DisableRandomForXTurns = m.DisableRandomForXTurns;
        this.ReduceXPower = m.ReduceXPower;
        this.ReducePowerForXTurns = m.ReducePowerForXTurns;
        this.BlockStrength = m.BlockStrength;

        this.HpForEnemy = m.HpForEnemy;

        this.BonusMaxHp = m.BonusMaxHp;
        this.BonusShield = m.BonusShield;
        this.BonusScavenging = m.BonusScavenging;
        this.BonusSalvaging = m.BonusSalvaging;
        this.HpRegen = m.BonusHpRegen;
        this.BonusDamageThermal = m.BonusDamageThermal;
        this.BonusDamageKinetic = m.BonusDamageKinetic;
        this.RevealHpAndModules = m.RevealHpAndModules;
        this.RevealDungeonInfo = m.RevealDungeonInfo;
        this.BonusArmor = m.BonusArmor;
        this.RegenPowerAfterKill = m.RegenPowerAfterKill;
        this.ProcessorOverclock = m.ProcessorOverclock;

        this.IsEnabled = true;
    }

    internal bool MatchDiceRolls(List<DiceRollData> diceRolls, bool onlyActive = true)
    {
        if (onlyActive)
        {
            if (diceRolls.Any(d => d.value == 1 && !d.isUsed) && DiceToActivate.HasFlag(DiceRoll.D1))
                return true;
            if (diceRolls.Any(d => d.value == 2 && !d.isUsed) && DiceToActivate.HasFlag(DiceRoll.D2))
                return true;
            if (diceRolls.Any(d => d.value == 3 && !d.isUsed) && DiceToActivate.HasFlag(DiceRoll.D3))
                return true;
            if (diceRolls.Any(d => d.value == 4 && !d.isUsed) && DiceToActivate.HasFlag(DiceRoll.D4))
                return true;
            if (diceRolls.Any(d => d.value == 5 && !d.isUsed) && DiceToActivate.HasFlag(DiceRoll.D5))
                return true;
            if (diceRolls.Any(d => d.value == 6 && !d.isUsed) && DiceToActivate.HasFlag(DiceRoll.D6))
                return true;
        }
        else
        {
            if (diceRolls.Any(d => d.value == 1) && DiceToActivate.HasFlag(DiceRoll.D1))
                return true;
            if (diceRolls.Any(d => d.value == 2) && DiceToActivate.HasFlag(DiceRoll.D2))
                return true;
            if (diceRolls.Any(d => d.value == 3) && DiceToActivate.HasFlag(DiceRoll.D3))
                return true;
            if (diceRolls.Any(d => d.value == 4) && DiceToActivate.HasFlag(DiceRoll.D4))
                return true;
            if (diceRolls.Any(d => d.value == 5) && DiceToActivate.HasFlag(DiceRoll.D5))
                return true;
            if (diceRolls.Any(d => d.value == 6) && DiceToActivate.HasFlag(DiceRoll.D6))
                return true;
        }
        return false;        
    }

    public string ModuleName;
    public ModuleKey Key;
    public string Desc;
    public SlotType Slot;
    public int Rank;
    public int SellValue = 5;
    public int MaxBoltsDrop = 10;
    public Module NextRank;
    public int PowerCost;
    public int UpgradeCost;
    public ModuleQuality Quality = ModuleQuality.Common;
    public ModuleUsages Usage;
    public AnimationType Animation;
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
    public int BonusMaxHp;
    public int BonusShield;
    /// <summary>
    /// 0-1 module drop chance
    /// </summary>
    public float BonusScavenging;
    /// <summary>
    /// 0-10 gold drop rate 1 = 100%
    /// </summary>
    public float BonusSalvaging;
    public int HpRegen;
    public int BonusDamageThermal;
    public int BonusDamageKinetic;
    public bool RevealHpAndModules;
    public bool RevealDungeonInfo;
    public int BonusArmor;
    public int RegenPowerAfterKill;
    /// <summary>
    /// ensures the sprcified value is always rolled on the dice/cpu output 0-6
    /// </summary>
    public int ProcessorOverclock;
}
