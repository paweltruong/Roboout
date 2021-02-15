using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// presents robo part in the robo prefab
/// </summary>
[ExecuteInEditMode]
public class ModuleVisualization : MonoBehaviour, IPaintJobPaintable
{
    //PTRU:all module ranks have same UI style for now
    
    [SerializeField] internal SlotType SlotType;
    [SerializeField] internal ModuleKey Key;
    //TODO:to be deleted(new approch is being developed)
    [SerializeField] PlugLayer placement = PlugLayer.InFront;
    [SerializeField] internal int slotPlacementIndex = 0;


    [Header("PaintJob")]
    [SerializeField] PaintJobGroups paintJobMapping;
    public PaintJobGroups PaintJobMapping { get { return paintJobMapping; } set { paintJobMapping = value; } }
    [SerializeField] PaintJobSpecificParts[] paintJobAdditionals;
    public PaintJobSpecificParts[] PaintJobAdditionals { get { return paintJobAdditionals; } set { paintJobAdditionals = value; } }

    internal Animator moduleAnimator;
    public bool IsBlocking;


    public UnityEvent onAnimationAttack1ContactFrame;
    public UnityEvent onAnimationKick1ContactFrame;
    public UnityEvent onAnimationBlock1ContactFrame;
    public UnityEvent onAnimationShotFrame;
    public UnityEvent onAnimationShotRecoiledFrame;

    private void Awake()
    {
        moduleAnimator = GetComponent<Animator>();   
    }

    public void OnAnimationShootFrame()
    {
        if (onAnimationShotFrame != null)
            onAnimationShotFrame.Invoke();
    }
    public void OnAnimationShootRecoiledFrame()
    {
        if (onAnimationShotRecoiledFrame != null)
            onAnimationShotRecoiledFrame.Invoke();
    }

    public void OnAnimationAttack1ContactFrame()
    {
        if (onAnimationAttack1ContactFrame != null)
            onAnimationAttack1ContactFrame.Invoke();
    }
    public void OnAnimationKick1ContactFrame()
    {
        if (onAnimationKick1ContactFrame != null)
            onAnimationKick1ContactFrame.Invoke();
    }
    public void OnAnimationBlock1ContactFrame()
    {
        if (onAnimationBlock1ContactFrame != null)
            onAnimationBlock1ContactFrame.Invoke();
    }
}
