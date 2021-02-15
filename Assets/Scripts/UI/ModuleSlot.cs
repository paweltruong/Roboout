using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// handles storing module in mainframe slot
/// </summary>
public class ModuleSlot : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI slotTypeLabel;
    [SerializeField] ModuleUI moduleUI;
    [SerializeField] SlotType type;

    internal bool isOccupied;
    internal bool isAvailable;




    private void Awake()
    {
        if (slotTypeLabel == null)
            Debug.LogError($"{nameof(slotTypeLabel)} not set up");
    }

    private void Start()
    {
        if (moduleUI == null)
            Debug.LogError("ModuleUI as child not found");
        slotTypeLabel.text = type.ToShortString();
    }

    /// <summary>
    /// place module in slot
    /// </summary>
    /// <param name="module"></param>
    internal void SetOccupied(ModuleInstance module)
    {
        isOccupied = true;
        moduleUI.Set(module, false, GameState.instance.playerData.Id);
        moduleUI.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }
    /// <summary>
    /// free to use, empty slot
    /// </summary>
    internal void SetUnocuppied()
    {
        isOccupied = false;
        moduleUI.gameObject.SetActive(false);
        gameObject.SetActive(true);

    }

    /// <summary>
    /// when mainframe does not have more slots, we need to hide(make unavailable other slots)
    /// </summary>
    internal void SetUnavailable()
    {
        isAvailable = false;
        isOccupied = false;
        gameObject.SetActive(false);
    }    

    /// <summary>
    /// set as available in mainframe, and reset its occupation status (later will be set called setting occupied or not)
    /// </summary>
    internal void SetAvailable()
    {
        isAvailable = true;
        isOccupied = false;
        moduleUI.gameObject.SetActive(false);
    }
}
