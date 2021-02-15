using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateTimeIntoOverride : MonoBehaviour
{
    TextMeshProUGUI textControl;

    private void Start()
    {
        textControl = GetComponent<TextMeshProUGUI>();
        if (textControl == null)
            Debug.LogError("TextTMP to overrride not set up");
    }

    public void DisplayDate()
    {
        textControl.text = $"{System.DateTime.Now.ToString("yyyy-MM-dd   hh:MM tt")}   Earth Time";
    }
}
