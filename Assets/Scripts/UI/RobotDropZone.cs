using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RobotDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RobotLoadout robo;
    [SerializeField] Color colorHostile = Color.white;
    [SerializeField] Color colorFriendly = Color.white;
    Image image;
    Color initialColor;

    /// <summary>
    /// added to class be testable when based on singleton
    /// </summary>
    public GameLogic gamelogic;

    public System.Guid OwnerId => robo.Id;
    public bool IsEnemy => robo.identity == RobotIdentity.Enemy;
    public bool IsAlly => robo.identity == RobotIdentity.PlayerAlly;
    public bool IsPlayer => robo.identity == RobotIdentity.Player;

    void Awake()
    {
        image = GetComponent<Image>();
        initialColor = image.color;
        HideHighlight();
    }

    void Start()
    {
        if (gamelogic == null)
        {
            gamelogic = GameLogic.Instance;
        }
    }

    public void ShowHighlight()
    {
        image.enabled = true;
    }
    public void HideHighlight()
    {
        image.enabled = false;
    }
    public void OnDrop(PointerEventData eventData)
    {
        gamelogic.TryUseModule(eventData, robo.Id);
        image.color = initialColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null)
        {
            Debug.LogWarning($"TODO:when does this occur? eventData:{eventData}");
        }
        else
        {
            var draggableModule = eventData.pointerDrag.GetComponent<ModuleDraggable>();
            if (draggableModule != null)
            {
                var moduleUI = draggableModule.GetComponent<ModuleUI>();
                if (moduleUI != null && GameState.instance != null)
                {
                    if (IsEnemy)
                    {
                        image.color = colorHostile;
                    }
                    else
                    {
                        image.color = colorFriendly;
                    }
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
