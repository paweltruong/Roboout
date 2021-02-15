using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class MainframeSetup_Base : MonoBehaviour, IPaintJobPaintable
{
    [Header("PaintJob")]
    [SerializeField] PaintJobGroups paintJobMapping;
    public PaintJobGroups PaintJobMapping { get { return paintJobMapping; } set { paintJobMapping = value; } }
    [SerializeField] PaintJobSpecificParts[] paintJobAdditionals;
    public PaintJobSpecificParts[] PaintJobAdditionals { get { return paintJobAdditionals; } set { paintJobAdditionals = value; } }

    //TODO:do we need this after ned approach
    [Header("Plugs")]
    public ModulePlugUI[] ReactorPlugs;
    public ModulePlugUI[] HeadPlugs;
    public ModulePlugUI[] ArmPlugs;
    public ModulePlugUI[] LegPlugs;
    public ModulePlugUI[] UtilityPlugs;

    [SerializeField]
    public ModuleVisualization[] ModuleVisualisations;

    internal Animator mainframeAnimator;

    internal void TriggerAnimationOnMainframe(string triggerName)
    {
        if (mainframeAnimator != null)
            mainframeAnimator.SetTrigger(triggerName);
    }

    internal void TriggerAnimationOnModule(string triggerName, ModuleKey key, int slotIndex)
    {

        if (ModuleVisualisations != null)
        {
            var modulesToActivate = ModuleVisualisations.Where(m => m != null && m.enabled && m.Key == key && m.slotPlacementIndex == slotIndex);
            foreach (var module in modulesToActivate)
            {
                if (module.moduleAnimator != null)
                {
                    module.moduleAnimator.SetTrigger(triggerName);
                }
            }
        }
    }
    internal void TriggerAnimationOnModules(string triggerName, SlotType slotType)
    {
        if (ModuleVisualisations != null)
        {
            var modulesToActivate = ModuleVisualisations.Where(m => m != null && m.enabled && m.SlotType == slotType);
            foreach (var module in modulesToActivate)
            {
                if (module.moduleAnimator != null)
                {
                    module.moduleAnimator.SetTrigger(triggerName);
                }
            }
        }
    }

    internal void TriggerAnimationOnAllModules(string triggerName)
    {
        if(ModuleVisualisations != null)
        {
            var activeModules = ModuleVisualisations.Where(m => m != null && m.enabled);
            foreach(var module in activeModules)
            {
                if(module.moduleAnimator != null)
                {
                    module.moduleAnimator.SetTrigger(triggerName);
                }
            }
        }
    }

    internal void SetAnimationOnMainframe(string paramName, bool value)
    {
        if (mainframeAnimator != null)
            mainframeAnimator.SetBool(paramName, value);
    }
    internal void SetAnimationOnAllModules(string paramName, bool value)
    {
        if (ModuleVisualisations != null)
        {
            var activeModules = ModuleVisualisations.Where(m => m != null && m.enabled);
            foreach (var module in activeModules)
            {
                if (module.moduleAnimator != null)
                {
                    module.moduleAnimator.SetBool(paramName, value);
                }
            }
        }
    }
}
