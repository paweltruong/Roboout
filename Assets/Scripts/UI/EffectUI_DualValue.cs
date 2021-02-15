using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// for presenting effect that has two values f.e  damage over time  (value1:damage value2:turns of debuff)
/// </summary>
public class EffectUI_DualValue : EffectUI
{
    [SerializeField] TextMeshProUGUI Value1Text;
    [SerializeField] TextMeshProUGUI Value2Text;


    public void SetDual(object value1, object value2)
    {
        Value1Text.text = value1?.ToString();
        Value2Text.text = value2.ToString();
    }
}
