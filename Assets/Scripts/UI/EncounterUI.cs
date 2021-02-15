using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class EncounterUI : MonoBehaviour
{
    /// <summary>
    /// if any of this breadcrums are unlocked the Encounter is available
    /// </summary>
    [SerializeField] internal EncounterUI[] Unlockers;
    [SerializeField] GameObject selectionBoarder;

    SpriteRenderer sr;


    [Header("DO NOT EDIT")]
    public EncounterType type;
    /// <summary>
    /// was this already visited and outcome was successful
    /// </summary>
    public bool isCompleted;
    /// <summary>
    /// was this scouted (details revealed)
    /// </summary>
    public bool isScouted;
    /// <summary>
    /// is this unlocked and reachable by player
    /// </summary>
    public bool isAvailable;
    /// <summary>
    /// can this be visited even after it was completed f.e shop
    /// </summary>
    public bool isPermanent;
    public bool isSelected;


    public UnityEvent<EncounterUI> onEncounterSelected;

    internal System.Guid Id;

    [Header("Icons")]
    [SerializeField] TextMeshProUGUI tutorialIcon;
    [SerializeField] TextMeshProUGUI UnknownIcon;
    [SerializeField] GameObject MarketIcon;
    [SerializeField] GameObject PowerStationIcon;
    [SerializeField] GameObject MainframeIcon;
    [SerializeField] GameObject WorkshopIcon;
    [SerializeField] GameObject BossIcon;
    [SerializeField] GameObject DangerIcon;
    [SerializeField] GameObject GoalIcon;

    internal void Set(EncounterInstance encounterInstance)
    {
        if (encounterInstance != null)
        {
            Id = encounterInstance.Id;
            type = encounterInstance.type;
            isCompleted = encounterInstance.isCompleted;
            isScouted = encounterInstance.isScouted;
            isAvailable = encounterInstance.isAvailable;
            isPermanent = encounterInstance.isPermanent;
            isCompleted = encounterInstance.isCompleted;
            isSelected = encounterInstance.isSelected;
            if (isSelected)
                SelectEncounter();
            else
                Deselect();
        }
    }

    private void Awake()
    {
        if (selectionBoarder == null)
            Debug.LogError($"{nameof(selectionBoarder)} not set");

        sr = GetComponent<SpriteRenderer>();

        if (onEncounterSelected == null)
            onEncounterSelected = new UnityEvent<EncounterUI>();
    }

    void Start()
    {
        UpdateIconsAndText();
    }

    public void SelectEncounter()
    {
        selectionBoarder.SetActive(true);
        isSelected = true;
        onEncounterSelected.Invoke(this);

        if (GameState.instance == null)
            Debug.LogError("GameState.instance not found");
        GameState.instance.mapData.SelectEncounter(Id);
    }

    void Update()
    {
        //TODO: refactor, update only on change
        UpdateBGColor();
        UpdateIconsAndText();
    }

    void UpdateIconsAndText()
    {
        DisplayIcons();
    }
    
    void DisplayIcons()
    {
        switch (type)
        {
            case EncounterType.Tutorial:
            case EncounterType.Blackmarket:
            case EncounterType.PowerStation:
            case EncounterType.MainframeFitting:
            case EncounterType.Workshop:
            case EncounterType.Boss:
            case EncounterType.Danger:
            case EncounterType.Goal:
                if (isScouted)
                {
                    ToggleIconsAndText(false,
                        type == EncounterType.Tutorial,
                        type == EncounterType.Blackmarket,
                        type == EncounterType.PowerStation,
                        type == EncounterType.MainframeFitting,
                        type == EncounterType.Workshop,
                        type == EncounterType.Boss,
                        type == EncounterType.Danger,
                        type == EncounterType.Goal);
                }
                else
                    ToggleIconsAndText(true, false, false, false, false, false, false, false, false);
                break;
            default:
                if (isAvailable)
                {
                    ToggleIconsAndText(true, false, false, false, false, false, false, false, false);
                }
                break;
        }
    }

    void ToggleIconsAndText(
        bool showUnknown,
        bool showTutorial,
        bool showMarket,
        bool showPowerStation,
        bool showMainframe,
        bool showWorkshop,
        bool showBoss,
        bool showDanger,
        bool showGoal)
    {
        UnknownIcon.gameObject.SetActive(showUnknown);
        tutorialIcon.gameObject.SetActive(showTutorial);
        MarketIcon.SetActive(showMarket);
        PowerStationIcon.SetActive(showPowerStation);
        MainframeIcon.SetActive(showMainframe);
        WorkshopIcon.SetActive(showWorkshop);
        BossIcon.SetActive(showBoss);
        DangerIcon.SetActive(showDanger);
        GoalIcon.SetActive(showGoal);
    }

    void UpdateBGColor()
    {
        if (isAvailable)
        {
            if (isPermanent)
                sr.color = Colors.Encounter_Permanent;
            else if (isCompleted)
                sr.color = Colors.Encounter_Completed;
            else
                sr.color = Colors.Encounter_Active;
        }
        else
        {
            sr.color = Colors.Encounter_Inactive;
        }
    }

    public void Deselect()
    {
        selectionBoarder.SetActive(false);
        isSelected = false;
    }
}
