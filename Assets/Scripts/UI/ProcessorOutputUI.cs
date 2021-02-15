using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// handles display of proc output in ModuleUI visualisation
/// </summary>
public class ProcessorOutputUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI value;

    private void Start()
    {
        if (value == null)
            Debug.LogError("processor output value control not set up");
    }

    public void Set(DiceRoll val)
    {
        value.text = val.ToProcessorOutputString();
        if (val <= 0)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
