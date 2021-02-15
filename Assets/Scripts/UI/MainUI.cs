using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used in  MainUI Scene
/// </summary>
public class MainUI : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI BoltsTextField;

    private void Start()
    {
        if (GameState.instance != null
            && GameState.instance.playerData != null)
            GameState.instance.playerData.onBoltsChanged += PlayerData_onBoltsChanged;

        if (BoltsTextField == null)
            Debug.LogError($"{nameof(BoltsTextField)} not set");
    }

    private void PlayerData_onBoltsChanged(RoboInstanceData sender, RoboInstanceDataEventArgs<float> e)
    {
        BoltsTextField.text = Mathf.FloorToInt(sender.Bolts).ToString();
    }
}
