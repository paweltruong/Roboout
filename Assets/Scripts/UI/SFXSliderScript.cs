using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO:refactor
public class SFXSliderScript : MonoBehaviour
{
    public Slider sfxSlider;

    void Start()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(MixerPatameters.SFXVolume, 0);
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnValueChanged);
        }
    }

    void OnValueChanged(float value)
    {
        GameMusic.instance.SetSfxVolume(value);           
    }

    private void OnDisable()
    {
        //float sfxVolume = 0;

        //GameMusic.instance.mixer.GetFloat("sfxVolume", out sfxVolume);

        //PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        //PlayerPrefs.Save();
        //Debug.Log("Player Pref reached");
    }
}
