using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public void GoToIntro()
    {
        SceneManager.LoadScene(SceneNames.Intro);
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(SceneNames.Menu);
    }

    public void GoToMap()
    {
        SceneManager.LoadScene(SceneNames.Map);
    }
    public void GoToBattle()
    {
        SceneManager.LoadScene(SceneNames.Battle);
    }
    public void GoToCredits()
    {
        SceneManager.LoadScene(SceneNames.Credits);
    }
    public void GoToOptions()
    {
        SceneManager.LoadScene(SceneNames.Options);
    }
    public void GoToHighscore()
    {
        SceneManager.LoadScene(SceneNames.Highscore);
    }


    public void GoToSceneToBeContinued()
    {
        if (GameState.instance != null)
            SceneManager.LoadScene(GameState.instance.sceneNameToContinue);
        else
            Debug.LogError("GameState not set up");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
