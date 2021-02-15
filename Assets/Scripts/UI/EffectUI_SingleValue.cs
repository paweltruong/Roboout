using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// for presenting effect value f.e amount of kinetic damage
/// </summary>
public class EffectUI_SingleValue : EffectUI
{
    [SerializeField] TextMeshProUGUI ValueText;

    public void Set(object value)
    {
        ValueText.text = value.ToString();
    }
}
