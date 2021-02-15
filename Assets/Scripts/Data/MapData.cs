using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//TODO: refactor , (extract methodsm and logic to GameLogic.Map.cs)
public class MapData
{
    internal EncounterInstance[] encounters;

    int generatedGoals = 0;
    int generatedMainframe = 0;
    int generatedMarket = 0;
    int generatedWorkshop = 0;
    int generatedPowerStation = 0;
    int generatedDangers = 0;
    int generatedBosses = 0;
    List<int> alreadyGeneratedIndexes = new List<int>();

    public EncounterInstance SelectedEncounter => encounters?.FirstOrDefault(e => e.isSelected);

    internal void InitializeMap(MapBlueprint blueprint)
    {
        Reset();

        if (blueprint.encounters == null || blueprint.encounters.Length <= 0)
            Debug.LogError("Map Blueprint does not have encoutners");

        StringBuilder sb_logMessage = new StringBuilder();
        encounters = new EncounterInstance[blueprint.encounters.Length];


        //first generate goals one the map
        var goals = blueprint.encounters.ToIndexed(i2e => i2e.Value.config == EncounterRandomConfig.Goal, alreadyGeneratedIndexes);

        if (goals.Any())
        {
            sb_logMessage.AppendLine($"Possible Goals (1) indexes: {string.Join(",", goals.Select(g => g.Key).ToArray())}");
            //TODO:
        }
        else
        {
            goals = blueprint.encounters.ToIndexed(i2e => i2e.Value.config == EncounterRandomConfig.OtherThanDangerPossibleGoal
                    || i2e.Value.config == EncounterRandomConfig.GoalOrMainframeFitting,
                    alreadyGeneratedIndexes);
            if (!goals.Any())
                Debug.LogError("no goals set in the map blueprint");
            else
            {
                sb_logMessage.AppendLine($"Possible Goals (2) indexes: {string.Join(",", goals.Select(g => g.Key).ToArray())}");
                while (generatedGoals < blueprint.minGoals)
                {
                    var goal = goals.ElementAt(Random.Range(0, goals.Count()));
                    var difficulty = goal.GetRandomDifficulty();
                    encounters[goal.Key] = new EncounterInstance(goal.Value, EncounterType.Goal, difficulty);
                    alreadyGeneratedIndexes.Add(goal.Key);
                    ++generatedGoals;
                    sb_logMessage.AppendLine($"Goal generated at [{goal.Key}]={goal.Value.name} ({encounters[goal.Key].difficulty}) ");
                }
            }
        }

        var tutorials = blueprint.encounters.ToIndexed(e => e.Value.config == EncounterRandomConfig.Tutorial, alreadyGeneratedIndexes);
        //tutorial
        //first
        if (!tutorials.Any())
            Debug.LogWarning("No tutorial in this map");
        else
        {
            var tut = tutorials.FirstOrDefault();
            encounters[tut.Key] = new EncounterInstance(tut.Value, EncounterType.Tutorial, tut.GetRandomDifficulty());
            encounters[tut.Key].isPermanent = true;
            encounters[tut.Key].isAvailable = true;
            encounters[tut.Key].isCompleted = true;//unlock neighbours of tutorial (tutorial is optional)
            encounters[tut.Key].isScouted = true;
            encounters[tut.Key].isSelected = true;
            alreadyGeneratedIndexes.Add(tut.Key);
        }

        //dangers - fixed place
        var dangers = blueprint.encounters.ToIndexed(e => e.Value.config == EncounterRandomConfig.Danger, alreadyGeneratedIndexes);
        if (dangers.Any())
        {
            foreach (var d in dangers)
            {
                DrawRandomDanger(blueprint, d);
            }
        }

        //bosses - fixed places
        var bosses = blueprint.encounters.ToIndexed(e => e.Value.config == EncounterRandomConfig.Boss, alreadyGeneratedIndexes);
        if (bosses.Any())
        {
            foreach (var b in bosses)
            {
                var res = DrawRandomBoss(blueprint, b);
                sb_logMessage.AppendLine($"Boss generated at [{res.Key}]={res.Value.name} ({encounters[res.Key].difficulty}) ");
            }
        }

        //bosses or default
        //TODO

        //mainframe fitting fixed
        var mainframeFixed = blueprint.encounters.ToIndexed(e => e.Value.config == EncounterRandomConfig.GoalOrMainframeFitting
                                                            || e.Value.config == EncounterRandomConfig.MainframeFitting
        , alreadyGeneratedIndexes);
        if (mainframeFixed.Any())
        {
            foreach (var mf in mainframeFixed)
            {
                if (generatedMainframe < blueprint.maxMainframe)
                {
                    DrawRandomMainframe(blueprint, mf);
                }
                else
                {
                    Debug.LogWarning($"Max mainframes mismatch with actual map configuration GeneratedCount>{blueprint.maxMainframe}");
                }
            }
        }

        //powerstationfitting
        //TODO:

        //workshopfitting
        //TODO:

        //market fitting
        //TODO:

        //other than danger and boss but not goal
        var shops = blueprint.encounters.ToIndexed(e => e.Value.config == EncounterRandomConfig.OtherThanDangerButNotGoal
                                                            || e.Value.config == EncounterRandomConfig.OtherThanDangerPossibleGoal
        , alreadyGeneratedIndexes);
        if (shops.Any())
        {
            foreach (var shop in shops)
            {
                List<int> availableToDraw = GetAvailableToDraw(blueprint,
                    includeMainframe: true, includeMarket: true,
                    includePowerStation: true, includeWorkshop: true);

                if (availableToDraw.Count > 0)
                {
                    var encounterType = (EncounterType)availableToDraw[Random.Range(0, availableToDraw.Count)];

                    DrawBasedOnEncounterType(blueprint, shop, encounterType);
                }
                else//nothing to draw from maxed out
                {
                    Debug.LogWarning($"Shops and pickups maxed out ignoring draw");
                    break;
                }
            }
        }


        //rest random
        var restRandom = blueprint.encounters.ToIndexed(e => e.Value.config == EncounterRandomConfig.Default
        , alreadyGeneratedIndexes);
        if (restRandom.Any())
        {
            foreach (var randomEncounter in restRandom)
            {
                List<int> availableToDraw = GetAvailableToDraw(blueprint,
                    includeMainframe: true, includeMarket: true,
                    includePowerStation: true, includeWorkshop: true,
                    includeDanger: true, includeBoss: true);

                if (availableToDraw.Count > 0)
                {
                    var encounterType = (EncounterType)availableToDraw[Random.Range(0, availableToDraw.Count)];

                    DrawBasedOnEncounterType(blueprint, randomEncounter, encounterType);
                }
                else//nothing to draw from maxed out
                {
                    Debug.LogWarning($"Default maxed out ignoring draw");
                    break;
                }
            }
        }

        //Set unlocker chain
        var indexedEncounterInstances = encounters.ToIndexed();
        foreach (var encounterInstance in indexedEncounterInstances)
        {
            //list of unlockers but as blueprints not indexed
            var unlockerBlueprints = blueprint.encounters[encounterInstance.Key].Unlockers;
            //get indexes of unlockers
            var indexedUnlockers = blueprint.encounters.ToIndexed(e => unlockerBlueprints.Contains(e.Value));
            encounterInstance.Value.UnlockerIndexes.AddRange(indexedUnlockers.Select(iu => iu.Key));
        }

        //Set starting node
        encounters[blueprint.startingEncounterIndex].isAvailable = true;
        UpdateAvailability();

        //validate map generation
        if (alreadyGeneratedIndexes.Count != blueprint.encounters.Length)
            Debug.LogError("Error during generation od level");

        Debug.Log($"Map generated:/r/n{sb_logMessage.ToString()}");
    }

    void Reset()
    {
        encounters = null;
        generatedGoals = 0;
        generatedMainframe = 0;
        generatedMarket = 0;
        generatedWorkshop = 0;
        generatedPowerStation = 0;
        generatedDangers = 0;
        generatedBosses = 0;
        alreadyGeneratedIndexes = new List<int>();
    }

    List<int> GetAvailableToDraw(MapBlueprint blueprint,
        bool includeMainframe = false, bool includeMarket = false,
        bool includeWorkshop = false, bool includePowerStation = false,
        bool includeDanger = false, bool includeBoss = false)
    {
        var availableToDraw = new List<int>();
        if (generatedMainframe < blueprint.maxMainframe)
            availableToDraw.Add((int)EncounterType.MainframeFitting);
        if (generatedMarket < blueprint.maxMarket)
            availableToDraw.Add((int)EncounterType.Blackmarket);
        if (generatedWorkshop < blueprint.maxWorkshop)
            availableToDraw.Add((int)EncounterType.Workshop);
        if (generatedPowerStation < blueprint.maxPowerstation)
            availableToDraw.Add((int)EncounterType.PowerStation);
        if (includeDanger)
            availableToDraw.Add((int)EncounterType.Danger);
        if (includeBoss)
            availableToDraw.Add((int)EncounterType.Boss);

        return availableToDraw;
    }

    public void UpdateAvailability()
    {
        List<int> nodesProcessed = new List<int>();
        var availableEncountersIndexed = encounters.ToIndexed(e => e.Value.isAvailable && e.Value.isCompleted);
        if (availableEncountersIndexed != null)
        {
            foreach (var availableEncounter in availableEncountersIndexed)
            {
                UnlockRecursively(availableEncounter, ref nodesProcessed);
            }
        }
    }

    void UnlockRecursively(KeyValuePair<int, EncounterInstance> availableEncounter, ref List<int> nodesProcessed)
    {
        if (!nodesProcessed.Contains(availableEncounter.Key))
        {
            var nodesThatAreUnlockedByThis = encounters.ToIndexed(e => e.Value.UnlockerIndexes.Contains(availableEncounter.Key));
            foreach (var nodeToUnlock in nodesThatAreUnlockedByThis)
            {
                nodeToUnlock.Value.isAvailable = true;
            }
            nodesProcessed.Add(availableEncounter.Key);
        }
    }

    private void DrawBasedOnEncounterType(MapBlueprint blueprint, KeyValuePair<int, EncounterBlueprint> encounterBlueprint, EncounterType encounterType)
    {
        switch (encounterType)
        {
            case EncounterType.MainframeFitting:
                DrawRandomMainframe(blueprint, encounterBlueprint);
                break;
            case EncounterType.PowerStation:
                DrawRandomPowerStation(blueprint, encounterBlueprint);
                break;
            case EncounterType.Workshop:
                DrawRandomWorkshop(blueprint, encounterBlueprint);
                break;
            case EncounterType.Blackmarket:
                DrawRandomMarket(blueprint, encounterBlueprint);
                break;
            case EncounterType.Boss:
                DrawRandomBoss(blueprint, encounterBlueprint);
                break;
            case EncounterType.Danger:
                DrawRandomDanger(blueprint, encounterBlueprint);
                break;
            default:
                Debug.LogError("Unsupported encouter type when generating");
                break;
        }
    }

    private void DrawRandomDanger(MapBlueprint blueprint, KeyValuePair<int, EncounterBlueprint> d)
    {
        var difficulty = d.GetRandomDifficulty();
        var randomDangerOnDifficulty = GetRandomDangerBlueprint(blueprint, difficulty);
        encounters[d.Key] = new EncounterInstance(d.Value, EncounterType.Danger, difficulty, randomDangerOnDifficulty);
        alreadyGeneratedIndexes.Add(d.Key);
        ++generatedDangers;
    }

    KeyValuePair<int, EncounterBlueprint> DrawRandomBoss(MapBlueprint blueprint, KeyValuePair<int, EncounterBlueprint> b)
    {
        var difficulty = b.GetRandomDifficulty();
        var randomDangerOnDifficulty = GetRandomBossBlueprint(blueprint, difficulty);
        encounters[b.Key] = new EncounterInstance(b.Value, EncounterType.Boss, difficulty, randomDangerOnDifficulty);
        alreadyGeneratedIndexes.Add(b.Key);
        ++generatedBosses;
        return b;
    }

    /// <summary>
    /// draws random mainframe on drawed difficulty on index of encounter array
    /// </summary>
    /// <param name="blueprint"></param>
    /// <param name="mf"></param>
    private void DrawRandomMainframe(MapBlueprint blueprint, KeyValuePair<int, EncounterBlueprint> mf)
    {
        var difficulty = mf.GetRandomDifficulty();
        var randomMainframeOnDifficulty = GetRandomMainframeBlueprint(blueprint, difficulty);
        encounters[mf.Key] = new EncounterInstance(mf.Value, EncounterType.MainframeFitting, difficulty, randomMainframeOnDifficulty);
        alreadyGeneratedIndexes.Add(mf.Key);
        ++generatedMainframe;
    }

    private void DrawRandomPowerStation(MapBlueprint blueprint, KeyValuePair<int, EncounterBlueprint> indexedEncounter)
    {
        var difficulty = indexedEncounter.GetRandomDifficulty();
        var randomPowerStationOnDifficulty = GetRandomPowerStationBlueprint(blueprint, difficulty);
        encounters[indexedEncounter.Key] = new EncounterInstance(indexedEncounter.Value, EncounterType.PowerStation, difficulty, randomPowerStationOnDifficulty);
        alreadyGeneratedIndexes.Add(indexedEncounter.Key);
        ++generatedPowerStation;
    }

    private void DrawRandomWorkshop(MapBlueprint blueprint, KeyValuePair<int, EncounterBlueprint> indexedEncounter)
    {
        var difficulty = indexedEncounter.GetRandomDifficulty();
        var randomWorkshopOnDifficulty = GetRandomWorkshopBlueprint(blueprint, difficulty);
        encounters[indexedEncounter.Key] = new EncounterInstance(indexedEncounter.Value, EncounterType.Workshop, difficulty, randomWorkshopOnDifficulty);
        alreadyGeneratedIndexes.Add(indexedEncounter.Key);
        ++generatedWorkshop;
    }

    private void DrawRandomMarket(MapBlueprint blueprint, KeyValuePair<int, EncounterBlueprint> indexedEncounter)
    {
        var difficulty = indexedEncounter.GetRandomDifficulty();
        var randomMarketOnDifficulty = GetRandomMarketBlueprint(blueprint, difficulty);
        encounters[indexedEncounter.Key] = new EncounterInstance(indexedEncounter.Value, EncounterType.Blackmarket, difficulty, randomMarketOnDifficulty);
        alreadyGeneratedIndexes.Add(indexedEncounter.Key);
        ++generatedMarket;
    }

    /// <summary>
    /// returns random  blueprint Danger for spacific difficulty range
    /// </summary>
    /// <param name="mapBlueprint"></param>
    /// <param name="difficulty"></param>
    /// <returns></returns>
    DangerBlueprint GetRandomDangerBlueprint(MapBlueprint mapBlueprint, EncounterDifficulty difficulty)
    {
        switch (difficulty)
        {
            //TODO:
            default:
                if (mapBlueprint.dangers_D01 == null || mapBlueprint.dangers_D01.Length <= 0)
                    Debug.LogError("danger01 BP is not set");
                return mapBlueprint.dangers_D01[Random.Range(0, mapBlueprint.dangers_D01.Length)];
        }
    }

    BossBlueprint GetRandomBossBlueprint(MapBlueprint mapBlueprint, EncounterDifficulty difficulty)
    {
        switch (difficulty)
        {
            //TODO:
            default:
                if (mapBlueprint.bosses_D01 == null || mapBlueprint.bosses_D01.Length <= 0)
                    Debug.LogError("bosses01 BP is not set");
                return mapBlueprint.bosses_D01[Random.Range(0, mapBlueprint.bosses_D01.Length)];
        }
    }
    Mainframe GetRandomMainframeBlueprint(MapBlueprint mapBlueprint, EncounterDifficulty difficulty)
    {
        switch (difficulty)
        {
            //TODO:
            default:
                if (mapBlueprint.mainframe_D01 == null || mapBlueprint.mainframe_D01.Length <= 0)
                    Debug.LogError("mainframe01 BP is not set");
                return mapBlueprint.mainframe_D01[Random.Range(0, mapBlueprint.mainframe_D01.Length)];
        }
    }
    PowerStation GetRandomPowerStationBlueprint(MapBlueprint mapBlueprint, EncounterDifficulty difficulty)
    {
        switch (difficulty)
        {
            //TODO:
            default:
                if (mapBlueprint.powerStation_DXX == null || mapBlueprint.powerStation_DXX.Length <= 0)
                    Debug.LogError("powerstation BP is not set");
                return mapBlueprint.powerStation_DXX[Random.Range(0, mapBlueprint.powerStation_DXX.Length)];
        }
    }
    Workshop GetRandomWorkshopBlueprint(MapBlueprint mapBlueprint, EncounterDifficulty difficulty)
    {
        switch (difficulty)
        {
            //TODO:
            default:
                if (mapBlueprint.workszop_DXX == null || mapBlueprint.workszop_DXX.Length <= 0)
                    Debug.LogError("workshop BP is not set");
                return mapBlueprint.workszop_DXX[Random.Range(0, mapBlueprint.workszop_DXX.Length)];
        }
    }
    Market GetRandomMarketBlueprint(MapBlueprint mapBlueprint, EncounterDifficulty difficulty)
    {
        switch (difficulty)
        {
            //TODO:
            default:
                if (mapBlueprint.market_D01 == null || mapBlueprint.market_D01.Length <= 0)
                    Debug.LogError("market BP is not set");
                return mapBlueprint.market_D01[Random.Range(0, mapBlueprint.market_D01.Length)];
        }
    }

    public void SelectEncounter(System.Guid id)
    {
        if (encounters != null)
        {
            foreach (var e in encounters)
            {
                if (e != null)
                {
                    if (e.Id == id)
                        e.isSelected = true;
                    else
                        e.isSelected = false;
                }
            }
        }
    }

    public void DeselectEncounter(System.Guid id)
    {
        if (encounters != null)
        {
            var found = encounters.FirstOrDefault(e => e != null && e.Id == id);
            if (found != null)
                found.isSelected = false;
        }
    }
}
