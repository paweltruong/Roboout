
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// manages visual presentation of combat on client
/// </summary>
public interface ICombatSceneLogic
{
    List<RobotLoadout> Robots { get; }
    Transform HandCardObjectPool { get; }
    bool IsDebug { get; }
    IEnumerable<EnemyBlueprint> DebugEnemyBlueprints { get; }

    void Initialize();
    void ShowVictory();
    void ShowDefeat();
    void ShowPreBattleUI();
    void ShowContinueBattleUI();
    void RefreshCurrentMoveUI();
    void RefreshDicesUI();
    void ToggleNextTurnUI(bool visible);
    void HideDice(int diceIndex);
    void HideAllUI();
    void SetBattleHint(string message);
    RoboInstanceData SpawnPlayer();
    RoboInstanceData SpawnEnemy(EnemyBlueprint enemyBlueprint);

    /// <summary>
    /// hide all modules from Hand panel
    /// </summary>
    void ClearHandUI();
    void RefreshAvailableModulesUI();

    /// <summary>
    /// <param name="delayBefore">wait before invoking action</param>
    /// <param name="delayAfter">wait after invoking action</param>
    /// <param name="action">main action to invoke</param>
    /// <param name="onFinishedCallback">called after action and delayAfter</param>
    /// </summary>
    void StartCoroutine(float delayBefore, float delayAfter, System.Action action, System.Action onFinishedCallback);
}
