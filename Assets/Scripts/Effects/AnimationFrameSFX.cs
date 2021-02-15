using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// sound that will be triggered by animator event (on ModuleVisualization)
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AnimationFrameSFX : MonoBehaviour
{
    [SerializeField] string sfxName;
    [SerializeField] AudioClip[] clipPool;
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogWarning($"no audio source in {name}");
    }
    public void PlaySFX()
    {
        if (clipPool != null && clipPool.Length > 0)
        {
            var randomClip = clipPool[Random.Range(0, clipPool.Length)];
            audioSource.PlayOneShot(randomClip);
        }
        else
            Debug.LogWarning($"no audio clip in {name}");
    }
}
