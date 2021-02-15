using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

//TODO:refactor order & sort prop,fields, events (CodeMaid?)

/// <summary>
/// Contains data/fields/state of current battle
/// Also have methods to set and manipulate context of the battle (but overal logic is in GameLogic)
/// </summary>
public class BattleData
{
    GameState gameState;
    IDiceRollerService diceRollerService;

    internal bool inProgress;
    internal bool hasStarted;
    internal bool hasFinished;
    internal bool won;
    internal int currentTurn;
    internal string name;



    public int CurrentMoveRobotIndex;
    public List<RoboMoveStatus> RobotsToMove = new List<RoboMoveStatus>();
    public RoboInstanceData CurrentMoveRobotData => AllFighters.FirstOrDefault(r => r.Id == RobotsToMove[CurrentMoveRobotIndex].Id);
    public RoboMoveStatus CurrentMove => RobotsToMove[CurrentMoveRobotIndex];
    public bool CurrentMoveRobotIsPlayer => CurrentMoveRobotData.Identity == RobotIdentity.Player;

    internal List<DiceRollData> diceRolls = new List<DiceRollData>();
    public IEnumerable<ModuleInstance> ModulesAtHand => CurrentMoveRobotData?.AllEquippedModules.Where(m => m.MatchDiceRolls(diceRolls));

    public event EventHandler onDiceRolled;
    public event RoboInstanceDataEventHandler onRobotKilled;

    public event ValueEventHandler<int> onBeginMovePhase;
    /// <summary>
    /// index of diceRolls
    /// </summary>
    public event ValueEventHandler<int> onDiceUsed;

    public event ValueEventHandler<System.Guid> onMoveChanged;
    public event ModuleUsageEventHandler onModuleUsedOn;

    /// <summary>
    /// gold
    /// </summary>
    public float BoltsDropped;
    public Module[] Loot;

    /// <summary>
    /// robot which is now moving in this turn
    /// </summary>
    internal System.Guid phaseOwnerId;
    /// <summary>
    /// phase of using active modules or passivemodules, passive modules are used if there is enough power for them and they were not pass on
    /// </summary>
    internal bool ActivePhase = true;
    /// <summary>
    /// if auto end was canceled => turn ended before timer reached 0, so coroutine should not execute NextTurn
    /// </summary>
    internal bool autoEndCanceled;

    List<RoboInstanceData> spawnedEnemies = new List<RoboInstanceData>();
    public IEnumerable<RoboInstanceData> Enemies => AllFighters.Where(r => r.Identity == RobotIdentity.Enemy);
    public RoboInstanceData playerRobot => AllFighters.First(r => r.Identity == RobotIdentity.Player);

    public List<RoboInstanceData> AllFighters { get; private set; } = new List<RoboInstanceData>();

    internal List<CombatLogRecord> combatLog = new List<CombatLogRecord>();


    public ModuleInstance FindModule(System.Guid moduleId, out RoboInstanceData owner)
    {
        owner = null;
        ModuleInstance result = null;
        foreach (var robot in AllFighters)
        {
            result = robot.GetModuleById(moduleId);
            if (result != null)
            {
                owner = robot;
                return result;
            }
        }
        return null;
    }

    public bool AreAllMoved => RobotsToMove.Any(r => !r.WaitingForMove);



    public BattleData()
    {
        diceRollerService = new DiceRollerService();
        if (gameState == null)
            gameState = GameState.instance;
    }

    public void DrawDices()
    {
        diceRolls.Clear();

        var currentMoveRobotData = CurrentMoveRobotData;

        var overclocksToUse = currentMoveRobotData.AllEquippedModules.Where(m => m.ProcessorOverclock > 0)
         .Select(m => m.ProcessorOverclock).ToList();

        diceRolls = new List<DiceRollData>();

        int diceCount = 0;
        var cores = currentMoveRobotData.mainframe.ProcessorCores;
        for (int i = 0; i < cores; ++i)
        {
            var dice = new DiceRollData();
            if (dice.RollAndTryToOverclock(diceRollerService, overclocksToUse.FirstOrDefault()))
            {
                //if succesfully overclocked then remove from dued overclocks list
                overclocksToUse.RemoveAt(0);
            }

            diceRolls.Add(dice);
            ++diceCount;
            onDiceRolled?.Invoke(this, new EventArgs());
        }

        //TODO: add additional cpu output(additional dices)
    }

    public void AddToCombatLog(string robotName, string message)
    {
        combatLog.Add(new CombatLogRecord(robotName, System.DateTime.Now, message));
    }
    
    public void AddSpawnedRobot(RoboInstanceData robot)
    {
        //check if robot not already on the list
        if (!AllFighters.Any(r => r.Id == robot.Id))
        {
            AllFighters.Add(robot);
            RobotsToMove.Add(new RoboMoveStatus { Id = robot.Id, Dead = false, Moved = false, Skipped = false });
            robot.onKilled += Robot_onKilled;
        }
    }

    private void Robot_onKilled(RoboInstanceData sender)
    {
        onRobotKilled?.Invoke(sender);
    }

    /// <summary>
    /// calculate and set amount of bolts/gold dropped from finished battle
    /// </summary>
    public void SetBoltsThatDropped()
    {
        var drop = 0f;
        foreach (var slainedEnemy in Enemies)
        {
            drop += slainedEnemy.DropBolts();
        }

        //apply bonus
        BoltsDropped = drop * playerRobot.TotalSalvage;
    }

    public void NextTurn()
    {
        hasStarted = true;
        ++currentTurn;
        foreach (var r in RobotsToMove)
            r.Reset();
        NextMove();
        Debug.Log($"Next turn, starting");
    }

    public void FinishMove()
    {
        var current = RobotsToMove[CurrentMoveRobotIndex];
        current.Moved = true;
        NextMove();
    }

    void NextMove()
    {
        var next = RobotsToMove.FirstOrDefault(r => r.WaitingForMove);
        if (next.Id != System.Guid.Empty)
        {
            CurrentMoveRobotIndex = RobotsToMove.IndexOf(next);
            phaseOwnerId = next.Id;
            onMoveChanged?.Invoke(next.Id);
            Debug.Log($"Now moves: {CurrentMoveRobotIndex}:{next.Id}");
        }
        else
            Debug.LogError("Invalid next robot to move");
    }



    internal void SetUpNewBattle()
    {
        if (gameState.mapData == null || gameState.mapData.SelectedEncounter == null)
            Debug.LogError($"{gameState.mapData} and {gameState.mapData.SelectedEncounter} not set");

        currentTurn = 0;
        name = gameState.mapData.SelectedEncounter.type.ToString();
        AllFighters.Clear();
        CurrentMoveRobotIndex = 0;
        inProgress = true;
        combatLog.Clear();
    }

    internal IEnumerable<RoboInstanceData> GetTargetsForCurrent()
    {
        switch (CurrentMoveRobotData.Identity)
        {
            case RobotIdentity.Enemy:
                return AllFighters.Where(r => !r.IsKilled && (r.Identity == RobotIdentity.Player || r.Identity == RobotIdentity.PlayerAlly));
            default:
                throw new NotImplementedException();
                break;
        }
    }


    internal void SkipMove()
    {
        CurrentMove.Skipped = true;
    }


    public void OnModuleUsed(RoboInstanceData source, ModuleInstance module, RoboInstanceData target)
    {
        onModuleUsedOn?.Invoke(source, module, target);
    }
    public void OnDiceUsed(int diceIndex)
    {
        onDiceUsed?.Invoke(diceIndex);
    }

    internal void SetDefeat()
    {
        hasFinished = true;
        won = false;
    }

    internal void SetVictory()
    {
        hasFinished = true;
        won = true;
    }
}
