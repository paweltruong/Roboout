using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO:refactor
public class SliderScript : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        
        slider.value = PlayerPrefs.GetFloat(MixerPatameters.MusicVolume, 0);
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnValueChanged);
        }
    }

    void OnValueChanged(float value)
    {
        GameMusic.instance.SetMusicVolume(value);
    }
}
