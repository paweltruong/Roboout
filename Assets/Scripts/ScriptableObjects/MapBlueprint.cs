using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// current level configuration, which encounters are in it, and configuration for their randomness
/// </summary>
[CreateAssetMenu(fileName ="MB000", menuName ="Map Blueprint")]
[System.Serializable]
public class MapBlueprint : ScriptableObject
{
    public string BlueprintName;
    public string Desc;
    public int startingEncounterIndex=0;
    public EncounterBlueprint[] encounters;
    public int minGoals;
    public int maxGoals;
    public int minMainframe;
    public int maxMainframe;
    public int minMarket;
    public int maxMarket;
    public int minWorkshop;
    public int maxWorkshop;
    public int minPowerstation;
    public int maxPowerstation;
    //enemy sets for speciffic difficulty level
    [Header("Enemy sets by Difficulty")]
    public DangerBlueprint[] dangers_D01;
    public DangerBlueprint[] dangers_D02;
    public DangerBlueprint[] dangers_D03;
    public DangerBlueprint[] dangers_D04;
    public DangerBlueprint[] dangers_D05;
    public DangerBlueprint[] dangers_D06;
    public DangerBlueprint[] dangers_D07;
    public DangerBlueprint[] dangers_D08;
    public DangerBlueprint[] dangers_D09;
    public DangerBlueprint[] dangers_D10;

    [Header("Bosses sets by Difficulty")]
    public BossBlueprint[] bosses_D01;
    public BossBlueprint[] bosses_D02;
    public BossBlueprint[] bosses_D03;
    public BossBlueprint[] bosses_D04;
    public BossBlueprint[] bosses_D05;
    public BossBlueprint[] bosses_D06;
    public BossBlueprint[] bosses_D07;

    [Header("Mainframe sets by Difficulty")]
    public Mainframe[] mainframe_D01;
    public Mainframe[] mainframe_D02;
    public Mainframe[] mainframe_D03;
    public Mainframe[] mainframe_D04;
    public Mainframe[] mainframe_D05;
    public Mainframe[] mainframe_D06;


    [Header("Powerstation sets by Difficulty")]
    public PowerStation[] powerStation_DXX;
    [Header("Workshop sets by Difficulty")]
    public Workshop[] workszop_DXX;


    [Header("Market sets by Difficulty")]
    public Market[] market_D01;
    public Market[] market_D02;
    public Market[] market_D03;
    public Market[] market_D04;
    public Market[] market_D05;
    public Market[] market_D06;
}
