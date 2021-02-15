using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains methods for testing GameState,PlayerData,etc
/// </summary>
public class GameStateDebugger : MonoBehaviour
{
    GameState gameState;

    void Start()
    {
        if (gameState == null)
            gameState = GameState.instance;
    }

   public void AddBolts(float amount)
    {
        if (gameState != null
            && gameState.playerData != null)
            gameState.playerData.AddBolts(amount);
    }
}
