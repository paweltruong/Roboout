using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Contains all the game state, options, player data, progress, battleData, etc
/// </summary>
public class GameState : MonoBehaviour
{
    [Header("State")]
    [SerializeField]
    internal bool gameInProgress;
    /// <summary>
    /// scripts and buttons for debugging purposes monitor this field
    /// </summary>
    [SerializeField] internal bool isDebugMode;


    [Header("Mandatory")]
    [SerializeField] internal MapBlueprint[] levels;
    [SerializeField] SetEffect[] supportedSets;
    [SerializeField] internal AssetRepository assetRepo;


    [Header("Settings")]
    /// <summary>
    /// When there are no possible move for player automatically end turn after {AutoEndTurnWaitTime}
    /// </summary>
    public bool autoEndTurn = true;
    /// <summary>
    /// seconds until auto end turn if AutoEndTurn is enabled
    /// </summary>
    public float autoEndTurnWaitTime = 2f;
    /// <summary>
    /// delay(in seconds) before AI move, so that player can see what is going on, which modules was available for enemy etc., 
    /// </summary>
    public float aiDecicionWaitTime = 2f;
    /// <summary>
    /// seconds after AI makes decision and its move is playing out, so player can assess damages and process what has happened before his move
    /// </summary>
    public float aiAfterMoveWaitTime = 2f;

    
    //singleton, to be replaced with Zenject?
    /// <summary>
    /// Probably in the future change to non static or di with Zenject
    /// </summary>
    public static GameState instance;

    /// <summary>
    /// Current level index (to initialize MapScene with)
    /// </summary>
    internal int levelIndex = 0;

    //game data
    //------------------------------------------
    /// <summary>
    /// stores player Robot data, atrributes, stats, modules, bolts(gold) etc
    /// </summary>
    internal PlayerData playerData;
    /// <summary>
    /// contains information about current map/level and its encounters
    /// </summary>
    internal MapData mapData;
    /// <summary>
    /// contains information about current battle encounter
    /// </summary>
    internal BattleData battleData;

    //Stats
    //------------------------------------------
    internal int battleCount = 0;
    //TODO:dmg done, heal received

    /// <summary>
    /// Name of the scene to continue (if returned to menu from specific scene, this scene should set this field with SceneToContinueFromMenu script)
    /// </summary>
    internal string sceneNameToContinue;

    public bool BattleInProgress => battleData != null && battleData.inProgress;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (levels == null || levels.Length <= 0)
            Debug.LogError($"{nameof(levels)} are not set up");
        if (supportedSets == null)
            Debug.LogError($"{nameof(supportedSets)} are not set up");
        if (assetRepo == null)
            Debug.LogError($"{nameof(assetRepo)} is not set up");

        Reset();
    }

    /// <summary>
    /// clears data for player, map, battle, and resets game progress
    /// </summary>
    public void Reset()
    {
        levelIndex = 0;
        playerData = new PlayerData();
        battleData = new BattleData();
        mapData = new MapData();
        gameInProgress = false;
    }
}
