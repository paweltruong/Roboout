using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MuzzleSpriteEffect : MonoBehaviour
{    
    [SerializeField] SpriteRenderer[] spritesRenderers;

    Color[] initialColor;//if someone called fade before original color was restore(fade in) so we need this
    float[] initialSteps;

    private void Awake()
    {
        if (spritesRenderers == null || spritesRenderers.Length == 0)
        {
            Debug.LogError("No sprites assigned to muzzle effect");
        }

        initialColor = new Color[spritesRenderers.Length];
        initialSteps = new float[spritesRenderers.Length];
        for (int i = 0; i < spritesRenderers.Length; ++i)
        {
            if (spritesRenderers[i] != null)
            {
                initialColor[i] = spritesRenderers[i].color;
            }
        }
        HideSprites();
    }


    public void MuzzleFadeIn(float framesToAppear)
    {

        for (int i = 0; i < spritesRenderers.Length; ++i)
        {
            if (framesToAppear == 0)
            {
                initialSteps[i] = initialColor[i].a;
            }
            else
            {
                initialSteps[i] = initialColor[i].a / framesToAppear;
            }
        }

        Debug.Log($"Start FadeIn with: IA[{string.Join(",", initialColor.Select(c => c.a))}] IS[{string.Join(",", initialSteps)}]");
        StartCoroutine(FadeIn(framesToAppear));
    }

    IEnumerator FadeIn(float fadeTime)
    {
        if (fadeTime > 0)
            SetAlpha0();
        else
            ResetAlpha();

        ShowSprites();
        for (int i = 0; i < fadeTime; ++i)
        {
            for (int j = 0; j < initialColor.Length; ++j)
            {
                var newColor = spritesRenderers[j].color;//starts from alpha 0
                newColor.a += (initialSteps[j] * i) / 255f;
                spritesRenderers[j].color = newColor;
            }
            yield return 0;
        }
    }

    private void SetAlpha0()
    {
        for (int i = 0; i < spritesRenderers.Length; ++i)
        {
            var newColor = initialColor[i];
            newColor.a = 0;
            spritesRenderers[i].color = newColor;
        }
    }
    private void ResetAlpha()
    {
        for (int i = 0; i < spritesRenderers.Length; ++i)
        {
            var newColor = initialColor[i];
            spritesRenderers[i].color = newColor;
        }
    }

    IEnumerator FadeOut(float fadeTime)
    {
        for (int i = 0; i < fadeTime; ++i)
        {
            for (int j = 0; j < initialColor.Length; ++j)
            {
                var newColor = initialColor[j];
                newColor.a = (initialColor[j].a - initialSteps[j] * i) / 255f;
                spritesRenderers[j].color = newColor;
            }
            yield return 0;
        }
        HideSprites();
    }

    public void MuzzleFadeOut(float framesToDisappear)
    {
        Debug.Log($"Start FadeOut with: IA[{string.Join(",", initialColor.Select(c => c.a))}] IS[{string.Join(",", initialSteps)}]");
        StartCoroutine(FadeOut(framesToDisappear));
    }

    void HideSprites()
    {
        for (int i = 0; i < spritesRenderers.Length; ++i)
        {
            spritesRenderers[i].gameObject.SetActive(false);
        }
    }
    void ShowSprites()
    {
        for (int i = 0; i < spritesRenderers.Length; ++i)
        {
            spritesRenderers[i].gameObject.SetActive(true);
        }
    }
}
