using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// when allowe scene to go to main menu and back
/// </summary>
public class SceneToContinueFromMenu : MonoBehaviour
{
    void Start()
    {

        if (GameState.instance != null)
            GameState.instance.sceneNameToContinue = SceneManager.GetActiveScene().name;
        else
            Debug.LogWarning("GameState not set up");
    }
}
