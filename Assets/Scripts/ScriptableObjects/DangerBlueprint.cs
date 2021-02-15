using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// standard  battle encounter configuration,
/// </summary>
[CreateAssetMenu(fileName = "DB000", menuName = "Danger Blueprint")]
[System.Serializable]
public class DangerBlueprint : ScriptableObject
{
    public EnemyBlueprint[] enemies;
}
