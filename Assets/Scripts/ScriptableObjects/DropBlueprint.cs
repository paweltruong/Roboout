using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// drop configuration of module that will be attached to enemy blueprint
/// </summary>
[CreateAssetMenu(fileName = "DRO000", menuName = "Drop Blueprint")]
[System.Serializable]
public class DropBlueprint : ScriptableObject
{
    public DropType type;
    public bool isUnique;
    public bool alwaysDrops;
    public float dropRate;

    [Header("Type:Module")]
    public Module module;
}
