using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// boss battle encounter configuration
/// </summary>
[CreateAssetMenu(fileName = "BB000", menuName = "Boss Blueprint")]
[System.Serializable]
public class BossBlueprint : ScriptableObject
{
    public EnemyBlueprint[] enemies;
}
