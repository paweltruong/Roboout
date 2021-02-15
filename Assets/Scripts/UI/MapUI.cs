using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// presentation logic for map scene (extract game logic to GameLogic.Map.cs)
/// </summary>
public class MapUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI encounterName;
    [SerializeField] GameObject exploreButton;
    [SerializeField] EncounterUI[] encounters;


    void Start()
    {
        if (encounters == null)
            Debug.LogError("No encounters on the map");

        ToggleExploreButton(false);
        AddListeners();

        UpdateMapData();
    }

    void BindLevels()
    {
        var gs = GameState.instance;
        if (gs == null || gs.mapData == null || gs.mapData.encounters == null)
            Debug.LogError("GameState or Map initialization property not set");

        if (encounters.Length != gs.mapData.encounters.Length)
            Debug.LogError("Encounters Count mismatch scene != mapData");

        gs.mapData.UpdateAvailability();

        for (int i = 0; i < encounters.Length; ++i)
        {
            encounters[i].Set(gs.mapData.encounters[i]);
        }

        //select node with a highlight
        var selected = gs.mapData.encounters.FirstOrDefault(e=>e!= null && e.isSelected);
        if (selected == null || (!selected.isPermanent && selected.isCompleted))
            ToggleExploreButton(false);
        else
            ToggleExploreButton(true);

        //apply scout effect
        if (gs.playerData.hasScout)
        {
            var scoutable = encounters.Where(e => e.isAvailable);
            foreach (var e in scoutable)
            {
                if (e != null)
                {
                    if (e.isAvailable)
                    {
                        e.isScouted = true;
                        var found = gs.mapData.encounters.FirstOrDefault(enc => enc != null && enc.Id == e.Id);
                        if (found != null)
                        {
                            found.isScouted = true;
                        }
                        else
                            Debug.LogError("Error while applying scout, encounter in mapData not found");
                    }
                }
            }
        }
    }
    internal void UpdateMapData()
    {
        BindLevels();
    }

    void AddListeners()
    {
        if (encounters != null)
        {
            for (int i = 0; i < encounters.Length; ++i)
            {
                encounters[i].onEncounterSelected.AddListener(SelectedEncounterChange);
            }
        } 
    }


    void RemoveListeners()
    {
        if (encounters != null)
        {
            for (int i = 0; i < encounters.Length; ++i)
            {
                encounters[i].onEncounterSelected.RemoveListener(SelectedEncounterChange);
            }
        } 
    }

    void Destroy()
    {
        RemoveListeners();
    }

    public void SelectedEncounterChange(EncounterUI encounter)
    {
        //deselect all encounters
        if (encounters != null)
            for (int i = 0; i < encounters.Length; ++i)
            {
                if (encounters[i].isSelected && encounters[i].GetInstanceID() != encounter.GetInstanceID())
                    encounters[i].Deselect();
            }

        //set up button
        if (encounter.isAvailable && (encounter.isPermanent || !encounter.isCompleted))
        {
            ToggleExploreButton(true);
        }
        else
        {
            ToggleExploreButton(false);
        }
    }
    
    public void SelectEncounter(EncounterUI encounter)
    {

    }

    void ToggleExploreButton(bool enable)
    {
        var selected = encounters.FirstOrDefault(e => e.isSelected);
        if(selected != null)
            encounterName.text = selected.ToMapEncounterName();
        exploreButton.SetActive(enable);
    }
}
