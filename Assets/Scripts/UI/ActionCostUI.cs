using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionCostUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI value;

    private void Start()
    {
        if (value == null)
            Debug.LogError("cost value control not set up");
    }

    public void Set(int val)
    {
        value.text = val.ToString();
        if (val <= 0)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
