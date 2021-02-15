using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// configuration for enemy robot
/// </summary>
[CreateAssetMenu(fileName = "ENE000", menuName = "Enemy Blueprint")]
[System.Serializable]
public class EnemyBlueprint : ScriptableObject
{
    public string EnemyName;
    public DropBlueprint[] drops;

    public Mainframe Mainframe;

    public Module[] reactors;
    public Module[] heads;
    public Module[] arms;
    public Module[] legs;
    public Module[] utilities;
    //TODO:does enemies have storage for modules?
    //public Module[] storage = new List<ModuleInstance>();
}
