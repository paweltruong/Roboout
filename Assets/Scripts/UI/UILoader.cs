using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// when want to use MainUI in the scene
/// </summary>
public class UILoader : MonoBehaviour
{
    void Start()
    {
        if (SceneManager.GetSceneByName(SceneNames.MainUIScene).isLoaded == false)
            SceneManager.LoadScene(SceneNames.MainUIScene, LoadSceneMode.Additive);
    }
}
