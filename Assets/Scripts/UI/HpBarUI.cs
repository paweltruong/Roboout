using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBarUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentPower;
    [SerializeField] Image barFill;
    [SerializeField] Image barDamage;
    [SerializeField] Image barHeal;
    [SerializeField] float barMinPosX = 3;
    [SerializeField] float barMaxWidth = 113;
    [SerializeField] Gradient gradientColor;
    [SerializeField] float effectCoroutineStep = .5f;


    private void Awake()
    {
        if (currentPower == null || barFill == null || barDamage == null)
            Debug.LogError("HpBarUI not set up");
    }

    void Start()
    {
        barDamage.enabled = false;
        barHeal.enabled = false;
    }

    public void Bind(RoboInstanceData roboInstanceData)
    {
        roboInstanceData.onPowerChanged += RoboInstanceData_onPowerChanged;
        UpdateCurrent(roboInstanceData);
    }

    private void RoboInstanceData_onPowerChanged(RoboInstanceData sender, RoboInstanceDataEventArgs<float> e)
    {
        UpdateCurrent(sender);
        AddHealthChangeEffect(sender, e.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roboInstanceData"></param>
    /// <param name="healthChange">positive is heal, negative is damage</param>
    void AddHealthChangeEffect(RoboInstanceData roboInstanceData, float healthChange)
    {
        var hpBeforeChange = roboInstanceData.CurrentPower - healthChange;
        var hpBeforeChangeAsPercent = Mathf.Abs(hpBeforeChange / roboInstanceData.MaxPower); 
        var hpAfterChangeAsPercent = Mathf.Abs((float)roboInstanceData.CurrentPower/(float)roboInstanceData.MaxPower); 
        var healthChangeAsPercent = Mathf.Abs(healthChange / roboInstanceData.MaxPower);
        if (healthChange > 0)
        {
            //heal
            barHeal.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barMaxWidth * hpAfterChangeAsPercent);
            barHeal.enabled = true;
            barFill.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barMaxWidth * hpBeforeChangeAsPercent);
            StartCoroutine(FadeOutHeal(barMaxWidth * hpAfterChangeAsPercent));
        }
        else
        {
            
            //damage
            barDamage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barMaxWidth * hpBeforeChangeAsPercent);
            barDamage.enabled = true;
            StartCoroutine(FadeOutDamage(barMaxWidth* hpAfterChangeAsPercent));
        }

    }
    IEnumerator FadeOutDamage(float targetBarWidth)
    {
        while(barDamage.rectTransform.rect.width > targetBarWidth)
        {
            barDamage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barDamage.rectTransform.rect.width - effectCoroutineStep);
            //Debug.Log($"Dmg FadeOut width: {barDamage.rectTransform.rect.width}");
            yield return new WaitForFixedUpdate();
        }
        barDamage.enabled = false;
        yield return null;
    }
    IEnumerator FadeOutHeal(float targetBarWidth)
    {
        while(barFill.rectTransform.rect.width < targetBarWidth)
        {
            barFill.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barFill.rectTransform.rect.width + effectCoroutineStep);
            //Debug.Log($"Heal FadeOut width: {barFill.rectTransform.rect.width}");
            yield return new WaitForFixedUpdate();
        }
        barHeal.enabled = false;
        yield return null;
    }

    void UpdateCurrent(RoboInstanceData roboInstanceData)
    {
        currentPower.text = $"{roboInstanceData.CurrentPower}/{roboInstanceData.MaxPower}";
        barFill.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barMaxWidth * roboInstanceData.HealthPercent);
        barFill.color = gradientColor.Evaluate(roboInstanceData.HealthPercent);

    }
}
