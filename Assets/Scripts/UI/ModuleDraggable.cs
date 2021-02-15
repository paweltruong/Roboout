using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// draggable script for module visualisation (cards)
/// </summary>
public class ModuleDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Transform parentToReturnTo = null;
    CanvasGroup canvasGroup;
    internal ModuleUI moduleUI;
    public bool AlreadyUsed;
    GameState gameState;
    GameLogic gameLogic;
    bool disableDrag;


    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        moduleUI = GetComponent<ModuleUI>();
    }

    void Start()
    {
        if (gameState == null)
            gameState = GameState.instance;
        if (gameLogic == null)
            gameLogic = GameLogic.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!gameState.battleData.CurrentMoveRobotIsPlayer)
        {
            //Debug.Log("drag AI");
            //AI phase do not allow drag
            disableDrag = true;
        }
        else
        {
            disableDrag = false;
            //Debug.Log("drag player");
            parentToReturnTo = this.transform.parent;
            this.transform.SetParent(this.transform.parent.parent);
            canvasGroup.blocksRaycasts = false;
            
            //Show dropzones/highlights
            var roboDropZones = FindObjectsOfType<RobotDropZone>();
            if (roboDropZones != null && roboDropZones.Length > 0)
            {
                RoboInstanceData owner;
                var module = gameState.battleData.FindModule(moduleUI.Id, out owner);
                if (module != null)
                {
                    //display highlights on valid target robots(those on which player can use dragged module)
                    foreach (var roboDropZone in roboDropZones)
                    {
                        if (module.AllowedTargets.Any(t => t == AllowedTargets.Enemy) && owner.IsPlayer)
                        {
                            if (roboDropZone.IsEnemy)
                                roboDropZone.ShowHighlight();
                        }
                        if (module.AllowedTargets.Any(t => t == AllowedTargets.Friendly) && owner.IsPlayer)
                        {
                            if (roboDropZone.IsAlly)
                                roboDropZone.ShowHighlight();
                        }
                        if (module.AllowedTargets.Any(t => t == AllowedTargets.Self) && owner.IsPlayer)
                        {
                            if (roboDropZone.OwnerId == owner.Id)
                                roboDropZone.ShowHighlight();
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Cannot find module {moduleUI.Id} in battle");
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!disableDrag)
            this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!disableDrag)
        {
            if (!AlreadyUsed)
                this.transform.SetParent(parentToReturnTo);
            canvasGroup.blocksRaycasts = true;

            HideDropZones();
        }
    }

    public void Used()
    {
        AlreadyUsed = true;
        moduleUI.gameObject.transform.SetParent(gameLogic.battleScene.HandCardObjectPool);
        moduleUI.gameObject.SetActive(false);
        canvasGroup.blocksRaycasts = true;

        HideDropZones();
    }

    void HideDropZones()
    {
        var roboDropZones = FindObjectsOfType<RobotDropZone>();
        if (roboDropZones != null && roboDropZones.Length > 0)
        {
            foreach (var roboDropZone in roboDropZones)
            {
                roboDropZone.HideHighlight();
            }
        }
    }
}
