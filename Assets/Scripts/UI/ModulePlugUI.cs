using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO:do we still need it after new approach of constructing robot
/// </summary>
public class ModulePlugUI : MonoBehaviour
{
    [SerializeField] internal SlotType SlotType;
    public ModuleVisualization module;
    [SerializeField] PlugLayer placement = PlugLayer.InFront;

}
