using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages visibility of the button should be put to scene logic
/// </summary>
public class ContinueButton : MonoBehaviour
{
    [SerializeField]
    GameObject continueButton;
    GameState gameState;
    private void Start()
    {
        gameState = FindObjectOfType<GameState>();
        if (gameState == null)
            Debug.LogError("GameState not found");
        if (continueButton == null)
            Debug.LogError("ContinueButton not set");

        continueButton.SetActive(gameState.gameInProgress);
    }
}
