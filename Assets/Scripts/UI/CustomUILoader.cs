using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomUILoader : MonoBehaviour
{
    public void  ToggleUIScene(string uiSceneName)
    {
        if (SceneManager.GetSceneByName(uiSceneName).isLoaded == false)
            SceneManager.LoadSceneAsync(uiSceneName, LoadSceneMode.Additive);
        else
            SceneManager.UnloadSceneAsync(uiSceneName);

    }

    public void LoadUIScene(string uiSceneName)
    {
        if (SceneManager.GetSceneByName(uiSceneName).isLoaded == false)
            SceneManager.LoadSceneAsync(uiSceneName, LoadSceneMode.Additive);
    }

    public void UnloadUIScene(string uiSceneName)
    {
        if (SceneManager.GetSceneByName(uiSceneName).isLoaded == true)
            SceneManager.UnloadSceneAsync(uiSceneName);
    }
}
