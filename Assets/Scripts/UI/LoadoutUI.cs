using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Responsible for managing loadout screen, and robot stats & attributes presentation
/// </summary>
public class LoadoutUI : MonoBehaviour
{
    [Header("Details")]
    [SerializeField] TextMeshProUGUI Mainframe;
    [SerializeField] TextMeshProUGUI PowerCapacity;
    [SerializeField] TextMeshProUGUI CPUCores;
    [SerializeField] TextMeshProUGUI Scavenging;
    [SerializeField] TextMeshProUGUI Salvaging;
    [SerializeField] TextMeshProUGUI Armor;
    [Header("Slots")]
    [SerializeField] ModuleSlot[] ReactorSlots = new ModuleSlot[4];
    [SerializeField] ModuleSlot[] HeadSlots = new ModuleSlot[4];
    [SerializeField] ModuleSlot[] ArmSlots = new ModuleSlot[4];
    [SerializeField] ModuleSlot[] LegSlots = new ModuleSlot[4];
    [SerializeField] ModuleSlot[] UtilitySlots = new ModuleSlot[4];
    [SerializeField] ModuleSlot[] StorageSlots = new ModuleSlot[4];

    private void Start()
    {
        if (Mainframe == null
            || PowerCapacity == null
            || CPUCores == null
            || Scavenging == null
            || Salvaging == null
            || Armor == null)
            Debug.LogError("Stat controls not set up");

        UpdateSlots();
        UpdateStats();
    }

    public void UpdateStats()
    {
        var playerData = GameState.instance.playerData;
        Mainframe.text = playerData.mainframe.MainframeName;

        var bonusHP = playerData.GetBonusHP();
        var bonusHPString = bonusHP > 0 ? $" (+{bonusHP})" : string.Empty;
        PowerCapacity.text = $"{playerData.CurrentPower}/{playerData.mainframe.BaseHp + bonusHP}{bonusHPString}";
        CPUCores.text = playerData.mainframe.ProcessorCores.ToString();//TODO: include bonuses from modules
        Scavenging.text = playerData.mainframe.BaseScavenging.GetFormattedPercent();//TODO: include bonuses from modules
        Salvaging.text = playerData.mainframe.BaseSalvaging.GetFormattedPercent();//TODO: include bonuses from modules
        Armor.text = playerData.mainframe.BaseArmor.ToString();//TODO: include bonuses from modules
    }

    public void UpdateSlots()
    {
        if (GameState.instance == null)
            Debug.LogError($"{nameof(GameState.instance)} is null");
        if(GameState.instance.playerData == null)
            Debug.LogError($"{nameof(GameState.instance.playerData)} is null");

        HideUnavailableSlots(GameState.instance.playerData.mainframe.ReactorSlots, ref ReactorSlots);
        HideUnavailableSlots(GameState.instance.playerData.mainframe.HeadSlots, ref HeadSlots);
        HideUnavailableSlots(GameState.instance.playerData.mainframe.ArmSlots, ref ArmSlots);
        HideUnavailableSlots(GameState.instance.playerData.mainframe.LegSlots, ref LegSlots);
        HideUnavailableSlots(GameState.instance.playerData.mainframe.UtilitySlots, ref UtilitySlots);
        HideUnavailableSlots(GameState.instance.playerData.mainframe.StorageSlots, ref StorageSlots);

        SetSlots(GameState.instance.playerData.reactors, ref ReactorSlots);
        SetSlots(GameState.instance.playerData.reactors, ref ReactorSlots);
        SetSlots(GameState.instance.playerData.heads, ref HeadSlots);
        SetSlots(GameState.instance.playerData.arms, ref ArmSlots);
        SetSlots(GameState.instance.playerData.legs, ref LegSlots);
        SetSlots(GameState.instance.playerData.utilities, ref UtilitySlots);
        SetSlots(GameState.instance.playerData.storage, ref StorageSlots);
    }
    
    void HideUnavailableSlots(int slotCapacity, ref ModuleSlot[] slots)
    {
        for (int i = slots.Length - 1; i >= 0; --i)
        {
            if (i > slotCapacity - 1)
            {
                slots[i].SetUnavailable();
            }
            else
                slots[i].SetAvailable();
        }
    }
    
    /// <summary>
    /// assign module into slot visualization
    /// </summary>
    /// <param name="modules"></param>
    /// <param name="slots"></param>
    void SetSlots(List<ModuleInstance> modules, ref ModuleSlot[] slots)
    {
        for (int i = 0; i < slots.Length; ++i)
        {
            if (i < modules.Count)
                slots[i].SetOccupied(modules[i]);
            else if (slots[i].isAvailable)
                slots[i].SetUnocuppied();
        }
    }
}
