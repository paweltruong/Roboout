using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Used for 'new game' button but also for confirmation when old game is in progress
/// </summary>
public class NewGameButton : MonoBehaviour
{
    GameState gameState;
    GameLogic gameLogic;

    [SerializeField]
    GameObject newGameDialog;
    NavigationManager navigationManager;
    
    private void Start()
    {
        if (gameLogic == null)
            gameLogic = GameLogic.Instance;

        gameState = FindObjectOfType<GameState>();
        if (gameState == null)
            Debug.LogError($"{nameof(gameState)} not found in scene");
        if(newGameDialog == null)
            Debug.LogError($"{nameof(newGameDialog)} not set");
        navigationManager = FindObjectOfType<NavigationManager>();
        if (navigationManager == null)
            Debug.LogError($"{nameof(navigationManager)} not found");
    }

    public void Click()
    {
        if(gameState != null)
        {
            if (gameState.gameInProgress)
            {
                newGameDialog.SetActive(true);
            }
            else
                StartNewGame();
        }
    }

    public void ClickConfirm()
    {
        StartNewGame();
    }
    
    void StartNewGame()
    {
        gameLogic.StartNewGame();
        navigationManager.GoToMap();
    }

}
