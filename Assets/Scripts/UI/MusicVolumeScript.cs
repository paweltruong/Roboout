using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicVolumeScript : MonoBehaviour
{

    public void SetMusicVolume(float musicVolume)
    {
        testowyMixer.SetFloat(MixerPatameters.MusicVolume, musicVolume);
    }

    public AudioMixer testowyMixer;


}
