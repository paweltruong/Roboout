using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour, ICombatSceneLogic
{

    [Header("Hand and cards")]
    [SerializeField] Transform handPanel;
    [SerializeField] Transform handCardObjectPool;
    public Transform HandCardObjectPool => handCardObjectPool;
    [SerializeField] ModuleUI[] handSlots;
    [Header("Spawning")]
    /// <summary>
    /// if debug spawn enemies not from selecter encounter config but from serialized field 'debugEnemyBlueprints'
    /// </summary>
    [SerializeField] bool isDebug;
    public bool IsDebug => isDebug;
    [SerializeField] GameObject RobotPrefab;
    [SerializeField] Transform[] PlayerSpawners;
    [SerializeField] Transform[] EnemySpawners;
    [SerializeField] public List<RobotLoadout> Robots { get; private set; } = new List<RobotLoadout>();


    [SerializeField] EnemyBlueprint[] debugEnemyBlueprints;
    public IEnumerable<EnemyBlueprint> DebugEnemyBlueprints => debugEnemyBlueprints;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI BattleStateHintText;
    [SerializeField] TextMeshProUGUI CombatLogText;
    [SerializeField] GameObject moduleCardPrefab;
    [SerializeField] DiceUI[] Dices;
    [SerializeField] GameObject FightButton;
    [SerializeField] GameObject EndTurnButton;
    [SerializeField] TextMeshProUGUI EndTurnButtonText;
    /// <summary>
    /// panel containing various controls involved with turn/end turn
    /// </summary>
    [SerializeField] GameObject TurnControls;
    [SerializeField] TextMeshProUGUI TurnValueText;
    [SerializeField] TextMeshProUGUI EncounterNameText;
    [SerializeField] TextMeshProUGUI WhoseTurnText;
    /// <summary>
    /// background color for buttons and labels when it is player's turn
    /// </summary>
    [SerializeField] Color PlayerTurnBgColor;
    /// <summary>
    /// background color for buttons and labels when it is enemie's turn
    /// </summary>
    [SerializeField] Color EnemyTurnBgColor;
    [SerializeField] Image TurnBgPanel;
    [SerializeField] BattleSummaryUI BattleSummaryCanvas;


    GameState gameState;
    GameLogic gameLogic;


    void Start()
    {
        if (gameState == null)
        {
            gameState = GameState.instance;
            if (gameState == null)
                Debug.LogError($"{gameState} not set");
        }

        if (GameMusic.instance != null)
            GameMusic.instance.PlayBattleMusic();
        else
            Debug.LogWarning("GameMusic.instance is not set up");

        if (RobotPrefab == null || PlayerSpawners == null || PlayerSpawners.Length == 0 || EnemySpawners == null || EnemySpawners.Length == 0)
            Debug.LogError("Spawning not set up");

        if (handPanel == null || handCardObjectPool == null || handSlots == null || handSlots.Length == 0)
            Debug.LogError("Hand and card slots not set up");

        if (IsDebug && (debugEnemyBlueprints == null || debugEnemyBlueprints.Length == 0))
            Debug.LogError("Debug enemy blueprint not set");

        if (BattleStateHintText == null || CombatLogText == null || moduleCardPrefab == null
        || Dices == null || Dices.Length == 0 || FightButton == null || EndTurnButton == null || EndTurnButtonText == null
        || TurnControls == null || TurnValueText == null || EncounterNameText == null || WhoseTurnText == null
        || TurnBgPanel == null
        || BattleSummaryCanvas == null)
            Debug.LogError("Scene UI not set up");


        if (gameLogic == null)
        {
            gameLogic = GameLogic.Instance;
            gameLogic.LoadBattle(this);
        }
    }

    private void Update()
    {
        //update combat log TODO:refactoring
        if (gameState != null && gameState.battleData != null && gameState.battleData.combatLog != null)
            CombatLogText.text = string.Join("\r\n", gameState.battleData.combatLog);

        if (Input.GetKeyUp(KeyCode.Q))
        {
            gameLogic.UseModuleAtHandIndex(0);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            gameLogic.UseModuleAtHandIndex(1);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            gameLogic.UseModuleAtHandIndex(2);
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            gameLogic.UseModuleAtHandIndex(3);
        }
    }

    public void Initialize()
    {
        BattleStateHintText.text = string.Empty;
        HideAllUI();
    }

    /// <summary>
    /// display victory screen after delay
    /// </summary>
    public void ShowVictory()
    {
        Debug.Log($"{nameof(BattleUI)}.{nameof(ShowVictory)}");

        StartCoroutine(ExecuteAfterTime(2, () =>
        {
            BattleSummaryCanvas.ShowVictory();
            BattleSummaryCanvas.AddBoltsDrop(gameState.battleData.BoltsDropped);
        }));
    }

    /// <summary>
    /// display defeat screen after delay
    /// </summary>
    public void ShowDefeat()
    {
        Debug.Log($"{nameof(BattleUI)}.{nameof(ShowDefeat)}");

        StartCoroutine(ExecuteAfterTime(2, () => BattleSummaryCanvas.ShowDefeat()));
    }

    public void SetBattleHint(string message)
    {
        BattleStateHintText.text = message;
    }

    public void RefreshCurrentMoveUI()
    {
        var currentRobotMoveStatus = gameState.battleData.RobotsToMove[gameState.battleData.CurrentMoveRobotIndex];
        var robot = gameState.battleData.AllFighters.FirstOrDefault(r => r.Id == currentRobotMoveStatus.Id);
        switch (robot.Identity)
        {
            default:
                TurnBgPanel.color = PlayerTurnBgColor;
                EndTurnButton.SetActive(true);
                break;
            case RobotIdentity.Enemy:
                TurnBgPanel.color = EnemyTurnBgColor;
                EndTurnButton.SetActive(false);
                break;
        }

        WhoseTurnText.text = System.String.Format(StringResources.UIText_WhoseTurnFormat, robot.RoboName);
    }

    public void HideDice(int diceIndex)
    {
        if (Dices.Length > diceIndex)
            Dices[diceIndex].Hide();
    }

    public void HideAllUI()
    {
        HideDices();
        HideBattleButtons();
        HideCardPanels();
        BattleStateHintText.text = System.String.Empty;
    }

    public void ShowContinueBattleUI()
    {
        RefreshDicesUI();
        RefreshAvailableModulesUI();
        ToggleNextTurnUI(true);
    }

    public void RefreshDicesUI()
    {
        //display dices
        for (int i = 0; i < Dices.Length; ++i)
        {
            if (i < gameState.battleData.diceRolls.Count)
            {
                Dices[i].Set(gameState.battleData.diceRolls[i]);
            }
            else
                Dices[i].Hide();
        }
    }

    public void ShowPreBattleUI()
    {
        HideDices();
        //show starting buttons
        ToggleNextTurnUI(false);
    }

    public RoboInstanceData SpawnPlayer()
    {
        var spawner = PlayerSpawners[Random.Range(0, PlayerSpawners.Length)];
        var newObj = Instantiate(RobotPrefab, spawner);
        var roboLoadout = newObj.GetComponent<RobotLoadout>();
        roboLoadout.Set(gameState.playerData, RobotIdentity.Player);
        Robots.Add(roboLoadout);
        return gameState.playerData;
    }
    public RoboInstanceData SpawnEnemy(EnemyBlueprint enemyBlueprint)
    {
        var gs = gameState;
        var spawner = EnemySpawners[Random.Range(0, EnemySpawners.Length)];
        var newObj = Instantiate(RobotPrefab, spawner);
        var roboLoadout = newObj.GetComponent<RobotLoadout>();
        var roboEnemyInstance = new RoboInstanceData();
        roboEnemyInstance.SetCompleteLoadout(enemyBlueprint.Mainframe, enemyBlueprint.reactors, enemyBlueprint.heads, enemyBlueprint.arms, enemyBlueprint.legs, enemyBlueprint.utilities);
        roboEnemyInstance.RoboName = enemyBlueprint.EnemyName;
        roboEnemyInstance.Identity = RobotIdentity.Enemy;
        roboLoadout.Set(roboEnemyInstance, RobotIdentity.Enemy);
        Robots.Add(roboLoadout);
        return roboEnemyInstance;
    }

    /// <summary>
    /// Run coroutine after X seconds
    /// </summary>
    /// <param name="delay">in seconds</param>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator ExecuteAfterTime(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);

        action?.Invoke();
    }

    private void HideCardPanels()
    {
        HandCardObjectPool.gameObject.SetActive(false);
        handPanel.gameObject.SetActive(false);
    }

    private void HideBattleButtons()
    {
        TurnControls.SetActive(false);
        FightButton.SetActive(false);
    }

    private void HideDices()
    {
        foreach (var diceUI in Dices)
        {
            if (diceUI != null)
                diceUI.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// called by button click
    /// </summary>
    public void NextTurn()
    {
        gameLogic.NextTurn();
    }

    /// <summary>
    /// called by GameLogic
    /// </summary>
    /// <param name="delay"></param>
    public void NextTurn(float delay)
    {
        Invoke(nameof(NextTurn), gameState.aiAfterMoveWaitTime);
    }

    public void RefreshAvailableModulesUI()
    {
        //for combat log
        var drawedModuleNames = new List<string>();


        for (int i = 0; i < handSlots.Length; ++i)
        {
            if (i < gameState.battleData.ModulesAtHand.Count())
            {
                var moduleInstance = gameState.battleData.ModulesAtHand.ElementAt(i);
                AssignModuleToHandSlot(i, moduleInstance);
                drawedModuleNames.Add(moduleInstance.ModuleName);
            }
        }

        handPanel.gameObject.SetActive(true);

        //TODO: extract some of it to GameLogic.battle?
        if (drawedModuleNames.Count > 0)
        {
            BattleStateHintText.text = StringResources.BattleHintText_FoundModules;
            EndTurnButtonText.text = StringResources.UIText_EndTurn;
            gameState.battleData.AddToCombatLog(gameState.playerData.RoboName,
                System.String.Format(StringResources.CombatText_FoundModules,
                string.Join(",", drawedModuleNames),
                string.Join(",", gameState.battleData.diceRolls.Select(d => d.value)))
                );
        }
        else
        {
            //jesli AI skipowal to nie nadpisuj komunikatu dla drugiej proby modulow? a moze nie powinno tu wogole wchodzic?
            if (!gameState.battleData.CurrentMove.Skipped)
                BattleStateHintText.text = StringResources.BattleHintText_NoModules;

            gameState.battleData.AddToCombatLog(gameState.playerData.RoboName,
                System.String.Format(StringResources.CombatText_NoModules,
                string.Join(",", gameState.battleData.diceRolls.Select(d => d.value)))
                );

            if (gameState.battleData.CurrentMoveRobotIsPlayer)
            {

                if (gameState.autoEndTurn)
                    StartCoroutine(AutoEndTurnCountdown(gameState.autoEndTurnWaitTime, gameState.battleData.currentTurn, gameState.battleData.CurrentMoveRobotIndex));
                else
                    EndTurnButtonText.text = StringResources.UIText_EndTurn;
            }
        }
    }

    public void ClearHandUI()
    {
        foreach (var s in handSlots)
            s.transform.SetParent(HandCardObjectPool);
    }

    private void AssignModuleToHandSlot(int slotIndex, ModuleInstance module)
    {
        var slot = handSlots[slotIndex];
        slot.Set(module, true, gameState.battleData.CurrentMoveRobotData.Id);
        slot.transform.SetParent(handPanel);
        slot.gameObject.SetActive(true);
        var draggable = slot.GetComponent<ModuleDraggable>();
        if (draggable != null)
        {
            draggable.AlreadyUsed = false;
        }
        else
            Debug.LogError($"Module {module.Id} is not draggable");
    }
   


    IEnumerator AutoEndTurnCountdown(float seconds, int currentTurn, int currentMoveIndex)
    {
        float counter = seconds;
        while (counter > 0)
        {
            EndTurnButtonText.text = string.Format(StringResources.UIText_EndTurnFormat, counter);
            yield return new WaitForSeconds(1);
            counter--;
        }
        //sprawdzamy czy ktos czegos nie kliknal albo automatyczie sie ruch juz nie zrobil zeby nie robic kolejnej tury kilka razy
        if (!gameState.battleData.autoEndCanceled)
        {
            if (currentTurn == gameState.battleData.currentTurn
            && currentMoveIndex == gameState.battleData.CurrentMoveRobotIndex)
            {
                NextTurn();
                gameState.battleData.autoEndCanceled = false;
            }
            else
                Debug.LogError($"AutoEndTurn something is not right! {currentTurn}=={gameState.battleData.currentTurn}, {currentMoveIndex}=={gameState.battleData.CurrentMoveRobotIndex}");
        }
        EndTurnButtonText.text = StringResources.UIText_EndTurn;
    }

    public void StartCoroutine(float delayBefore, float delayAfter, System.Action action, System.Action onFinishedCallback)
    {
        StartCoroutine(WaitAndAct(delayBefore, delayAfter, action, onFinishedCallback));
    }

    /// <summary>
    /// make ai think for some time so player can see what are the ai choices
    /// </summary>
    /// <param name="delayBefore"></param>
    /// <param name="delayAfter"></param>
    /// <param name="aiMoveFinishedCallback"></param>
    /// <returns></returns>
    IEnumerator WaitAndAct(float delayBefore, float delayAfter, System.Action action, System.Action onFinishedCallback)
    {
        float counter = delayBefore;
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
        }

        action?.Invoke();

        yield return new WaitForSeconds(delayAfter);
        onFinishedCallback?.Invoke();
    }

    public void ToggleNextTurnUI(bool visible)
    {
        FightButton.SetActive(!visible);
        TurnControls.SetActive(visible);
        RefreshNextTurnUI();
    }

    void RefreshNextTurnUI()
    {
        TurnValueText.text = gameState.battleData.currentTurn.ToString();
        if (gameState != null)
            EncounterNameText.text = gameState.battleData.name;
    }
}
