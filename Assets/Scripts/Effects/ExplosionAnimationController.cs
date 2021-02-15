using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class ExplosionAnimationController : MonoBehaviour
{
    public UnityEvent onExplosionMaxEffectFrame;
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void OnExplosionMaxEffext()
    {
        onExplosionMaxEffectFrame?.Invoke();
    }

    internal void PlayAnimation()
    {
        gameObject.SetActive(true);
        anim.Play(0);
    }
}
