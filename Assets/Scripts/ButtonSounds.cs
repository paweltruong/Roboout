using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    GameMusic gameMusic;
    public AudioClip hover;
    
    void Start()
    {
        gameMusic = FindObjectOfType<GameMusic>();
    }

    public void PlayHover()
    {
        if (gameMusic != null)
            gameMusic.PlaySFX(hover);
        else
            Debug.LogWarning("GameMusic component not found in scene");
    }
}
