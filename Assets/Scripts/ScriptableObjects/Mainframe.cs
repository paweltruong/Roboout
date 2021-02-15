using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// robot body configuration, how many modules of type can be installed, stats & attributes, max hp etc.
/// </summary>
[CreateAssetMenu(fileName ="MF000", menuName ="Robo Mainframe")]
public class Mainframe : ScriptableObject
{
    public string MainframeName;
    public string Desc;
    public int Rank;
    public int MaxBoltsDrop = 30;
    public MainframeType Type;
    [Header("Stats")]
    [Range(0, 1)]
    public float BaseScavenging;
    [Range(1, 10)]
    public float BaseSalvaging;
    public float BaseHp;
    public int BaseArmor;
    [Range(1,3)]
    public int ProcessorCores;
    public int StorageSlots;
    public int UtilitySlots;
    public int HeadSlots;
    public int ArmSlots;
    public int LegSlots;
    public int ReactorSlots; 
}
