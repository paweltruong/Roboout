using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// encounter configuration for map building
/// </summary>
[CreateAssetMenu(fileName = "EB000", menuName = "Encounter Blueprint")]
[System.Serializable]
public class EncounterBlueprint : ScriptableObject
{
    public EncounterRandomConfig config;
    public bool isRandom = true;
    public EncounterDifficulty minDifficulty;
    public EncounterDifficulty maxDifficulty;
    [Tooltip("Nody z mapy których aktywowanie/ukonczenie odblokowuje sciezke do tego")]
    public EncounterBlueprint[] Unlockers;
}
