using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class EncounterInstance
{
    internal System.Guid Id;
    internal EncounterType type;
    internal EncounterDifficulty difficulty;

    public bool isCompleted;
    /// <summary>
    /// is encounter is scouted and player can see detailed information about enemies etc.
    /// </summary>
    public bool isScouted;
    /// <summary>
    /// is encounter reachable by the player, is path to it available
    /// </summary>
    public bool isAvailable;
    /// <summary>
    /// can player visit this encounter even after finishing it once f.e shop with parts
    /// </summary>
    public bool isPermanent;
    public bool isSelected;

    /// <summary>
    /// will be set in MapData
    /// </summary>
    public List<int> UnlockerIndexes;

    public List<EnemyBlueprint> enemyBlueprints;

    /// <summary>
    /// is this encounter a battle type
    /// </summary>
    public bool IsBattleType => this.type == EncounterType.Tutorial
                || this.type == EncounterType.Danger
                || this.type == EncounterType.Boss;

    public EncounterInstance(EncounterBlueprint blueprint, EncounterType encounterType, EncounterDifficulty encounterDifficulty)
    {
        UnlockerIndexes = new List<int>();
        Id = Guid.NewGuid();
        isCompleted = false;
        isScouted = false;
        isAvailable = false;
        isPermanent = false;

        this.type = encounterType;
        this.difficulty = encounterDifficulty;
    }

    public EncounterInstance(EncounterBlueprint blueprint, EncounterType encounterType, EncounterDifficulty encounterDifficulty, DangerBlueprint randomDangerBlueprint)
        : this(blueprint, encounterType, encounterDifficulty)
    {
        this.enemyBlueprints = randomDangerBlueprint.enemies.ToList();
    }
    public EncounterInstance(EncounterBlueprint blueprint, EncounterType encounterType, EncounterDifficulty encounterDifficulty, BossBlueprint randomBossBlueprint)
        : this(blueprint, encounterType, encounterDifficulty)
    {
        this.enemyBlueprints = randomBossBlueprint.enemies.ToList();
    }
    public EncounterInstance(EncounterBlueprint blueprint, EncounterType encounterType, EncounterDifficulty encounterDifficulty, Mainframe randomMainframeBlueprint)
        : this(blueprint, encounterType, encounterDifficulty)
    {
    }
    public EncounterInstance(EncounterBlueprint blueprint, EncounterType encounterType, EncounterDifficulty encounterDifficulty, PowerStation randomPowerStationBlueprint)
        : this(blueprint, encounterType, encounterDifficulty)
    {
    }
    public EncounterInstance(EncounterBlueprint blueprint, EncounterType encounterType, EncounterDifficulty encounterDifficulty, Workshop randomWorkshopBlueprint)
        : this(blueprint, encounterType, encounterDifficulty)
    {
    }
    public EncounterInstance(EncounterBlueprint blueprint, EncounterType encounterType, EncounterDifficulty encounterDifficulty, Market randomMarketBlueprint)
        : this(blueprint, encounterType, encounterDifficulty)
    {
    }

    public void Initialize(EncounterBlueprint blueprint)
    {
    }
}
