using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// TODO:move from this script and MapScene to Menu Editor cheats
/// </summary>
public class MapDebugger : MonoBehaviour
{
    [SerializeField] GameObject[] debugElements;
    [SerializeField] MapUI mapUI;

    GameState gameState;

    void Start()
    {
        if (gameState == null)
            gameState = GameState.instance;

        if (mapUI == null)
            Debug.LogError("MapUI not set");

        if (!gameState.isDebugMode)
        {
            if (debugElements != null)
                foreach (var debugElementGO in debugElements)
                    debugElementGO.SetActive(false);
        }
    }

    public void CompleteSelectedEncounter()
    {
        Debug.Log(nameof(CompleteSelectedEncounter));
        if (GameState.instance != null)
        {
            var selectedEncounter = GameState.instance.mapData.encounters.FirstOrDefault(e => e != null && e.isSelected);
            {
                if (selectedEncounter != null)
                {
                    selectedEncounter.isAvailable = true;
                    selectedEncounter.isScouted = true;
                    selectedEncounter.isCompleted = true;
                    selectedEncounter.isSelected = false;
                    mapUI.UpdateMapData();
                }
            }
        }
    }


    public void FailSelectedEncounter()
    {
        //TODO
        Debug.Log(nameof(FailSelectedEncounter));
    }

    /// <summary>
    /// for testing scout ability
    /// </summary>
    public void RevealSelectedEncounter()
    {
        Debug.Log(nameof(RevealSelectedEncounter));
        if (GameState.instance != null)
        {
            var selectedEncounter = GameState.instance.mapData.encounters.FirstOrDefault(e => e != null && e.isSelected);
            {
                if (selectedEncounter != null)
                {
                    selectedEncounter.isAvailable = true;
                    selectedEncounter.isScouted = true;
                    mapUI.UpdateMapData();
                }
            }
        }
    }

    public void RevealAllEncounters()
    {
        Debug.Log(nameof(RevealAllEncounters));
        if (GameState.instance != null)
        {
            foreach (var e in GameState.instance.mapData.encounters)
            {
                if (e != null)
                {
                    e.isAvailable = true;
                    e.isScouted = true;
                }
            }
            mapUI.UpdateMapData();
        }
    }

    public void ToggleScoutAbillity(bool enabled)
    {
        gameState.playerData.hasScout = enabled;
        mapUI.UpdateMapData();
    }
}
