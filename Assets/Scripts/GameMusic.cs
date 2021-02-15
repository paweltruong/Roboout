using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;


public class GameMusic : MonoBehaviour
{
    public static GameMusic instance;
    public enum MusicType
    {
        Stopped,
        Menu,
        Battle,
    }


    /// <summary>
    /// During game interval between playing music scores in seconds
    /// </summary>
    float MusicIntervalInSeconds = 5f;
    float musicTimer = 0f;

    public AudioClip Menu;
    public AudioClip[] Battle;

    [SerializeField()]
    AudioSource PersitentMusic;

    [SerializeField()]
    AudioSource PersistentSoundFX;   

    internal MusicType currentMusicType = MusicType.Menu;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMainTheme();

    }

    public void PlayMainTheme()
    {
        currentMusicType = MusicType.Menu;
        PersitentMusic.clip = Menu;
        PersitentMusic.loop = true;
        PersitentMusic.Play();
        Debug.Log("Start playing music");
    }

    public void PlayBattleMusic()
    {
        Debug.Log("Play battle music");
        //TODO: pause menu music and then resume
        if (currentMusicType != MusicType.Battle)
        {
            currentMusicType = MusicType.Battle;
            PersitentMusic.loop = false;
            PlayRandomBattleMusic();
        }
    }

    private void Update()
    {
        if (currentMusicType == MusicType.Battle && !PersitentMusic.isPlaying)
        {
            if (musicTimer < MusicIntervalInSeconds)
                musicTimer += Time.deltaTime;
            else
                PlayRandomBattleMusic();
        }
        else if (currentMusicType == MusicType.Menu && !PersitentMusic.isPlaying)
            PlayMainTheme();
    }

    void PlayRandomBattleMusic()
    {
        musicTimer = 0;
        AudioClip randomSong;
        switch (currentMusicType)
        {
            default:
                randomSong = Battle[Random.Range(0, Battle.Length)];
                break;
        }

        PersitentMusic.clip = randomSong;
        PersitentMusic.Play();
    }
   
    public void StopMusic()
    {
        currentMusicType = MusicType.Stopped;
        PersitentMusic.Stop();
    }
    
    /// <summary>
    /// for interface sounds
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySFX(AudioClip clip)
    {
        PersistentSoundFX.clip = clip;
        PersistentSoundFX.Play();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;    
       
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //TODO:changed music type based on scene
    }

    public AudioMixer mixer;
 
    public void SetMusicVolume(float musicVolume)
    {
        mixer.SetFloat(MixerPatameters.MusicVolume, musicVolume);
        PlayerPrefs.SetFloat(MixerPatameters.MusicVolume, musicVolume);
        PlayerPrefs.Save();
    }
    public void SetSfxVolume(float sfxVolume)
    {
        mixer.SetFloat(MixerPatameters.SFXVolume, sfxVolume);
        PlayerPrefs.SetFloat(MixerPatameters.SFXVolume, sfxVolume);
        PlayerPrefs.Save();
    }
}