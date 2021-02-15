using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class Cheats
{
    [MenuItem("Roboout/DMGK 1 to player")]
    public static void DamagePlayer1()
    {
        ExecuteIfInBattle(() => GameLogic.Instance.DamagePlayerKinetic(1));
    }
    [MenuItem("Roboout/DMGK 3 to player")]
    public static void DamagePlayer3()
    {
        ExecuteIfInBattle(() => GameLogic.Instance.DamagePlayerKinetic(3));
    }
    [MenuItem("Roboout/Heal 3 to player")]
    public static void HealPlayer3()
    {
        ExecuteIfInBattle(() => GameLogic.Instance.HealPlayer(3));
    }
    [MenuItem("Roboout/DMGK 1 to enemy")]
    public static void DamageEnemy1()
    {
        ExecuteIfInBattle(() => GameLogic.Instance.DamageEnemyKinetic(1));
    }
    [MenuItem("Roboout/DMGK 3 to enemy")]
    public static void DamageEnemy3()
    {
        ExecuteIfInBattle(() => GameLogic.Instance.DamageEnemyKinetic(3));
    }
    [MenuItem("Roboout/Heal 3 to enemy")]
    public static void HealEnemy3()
    {
        ExecuteIfInBattle(() => GameLogic.Instance.HealEnemy(3));
    }

    static void ExecuteIfInBattle(Action action)
    {
        if (Application.isPlaying)
        {
            if (GameState.instance != null && GameState.instance.BattleInProgress)
                action?.Invoke();
            else
                Debug.LogError("Not in battle");
        }
        else
        {
            Debug.LogError("Not in play mode.");
        }
    }
}