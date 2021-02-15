
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// TODO in the future extract logic to another class and this will tranform into transport layer calling logic server for multiplayer games
/// </summary>
public partial class GameLogic : IGameLogic
{

    string gameId;
    GameState gameState;
    //to simplify calls inside methods
    AssetRepository Assets => gameState.assetRepo;
    PlayerData PlayerData => gameState.playerData;
    BattleData BattleData => gameState.battleData;
    MapData MapData => gameState.mapData;

    /// <summary>
    /// presentation layer of state and logic(is handling all visualizations and UI)
    /// </summary>
    internal ICombatSceneLogic battleScene;

       
    static GameLogic instance;
    /// <summary>
    /// Probably in the future change to non static or di with Zenject
    /// </summary>
    public static GameLogic Instance
    {
        get
        {
            if (instance == null)
                instance = new GameLogic();
            return instance;
        }
    }

    public GameLogic()
    {
        gameState = GameState.instance;
        if (gameState == null)
            Debug.LogError($"{gameState} is null");
    }

    public void StartNewGame()
    {
        gameId = GetHashString();
        gameState.Reset();
        gameState.gameInProgress = true;

        //Init PlayerData
        PlayerData.SetCompleteLoadout(Assets.defaultMainframe, Assets.defaultReactors, Assets.defaultHeads, Assets.defaultArms, Assets.defaultLegs);

        Debug.Log(PlayerData.PrintLoadout());
        
        //InitMap
        InitializeMapData();
    }


    /// <summary>
    /// Get unique hash
    /// </summary>
    /// <returns></returns>
    string GetHashString()
    {
        string hashString = string.Empty;
        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString() + Environment.MachineName + Environment.UserName));
            var hashedInputStringBuilder = new System.Text.StringBuilder(128);
            foreach (var b in hash)
                hashedInputStringBuilder.Append(b.ToString("X2"));
            hashString = hashedInputStringBuilder.ToString();
        }
        return hashString;
    }

    void InitializeMapData()
    {
        MapData.InitializeMap(gameState.levels[gameState.levelIndex]);
    }

    /// <summary>
    /// Player uses module when dropped on targetRobot
    /// </summary>
    /// <param name="dropEventData"></param>
    /// <param name="targetRobotId"></param>
    public void TryUseModule(PointerEventData dropEventData, System.Guid targetRobotId)
    {
        var draggableModule = dropEventData.pointerDrag.GetComponent<ModuleDraggable>();
        if (draggableModule != null)
        {
            var moduleUI = draggableModule.GetComponent<ModuleUI>();
            if (moduleUI != null)
            {
                if (TryUseModule(PlayerData, moduleUI.Id, targetRobotId))
                    draggableModule.Used();
            }
        }
    }
}