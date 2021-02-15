using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// handles presentatrion of dices / cpu output
/// </summary>
[ExecuteInEditMode]
public class DiceUI : MonoBehaviour
{

    [SerializeField] Color PlayerDiceColor;
    [SerializeField] Color EnemyDiceColor;
    [SerializeField] Color AllyDiceColor;
    [SerializeField] Color PlayerDiceTextColor;
    [SerializeField] Color EnemyDiceTextColor;
    [SerializeField] Color AllyDiceTextColor;
    [SerializeField] Image DiceBGColor;
    [SerializeField] GameObject OverclockedBackground;
    [SerializeField] GameObject AdditionalDiceBackground;
    [SerializeField] TextMeshProUGUI ValueText;
    int value;
    [Header("Debug")]
    [SerializeField] RobotIdentity Identity;
    [SerializeField] bool IsOverclocked;
    [SerializeField] bool IsAdditional;


    private void Update()
    {
        if (Application.isEditor)
            Redraw();
    }

    private void Redraw()
    {
        switch(Identity)
        {
            default:
                DiceBGColor.color = PlayerDiceColor;
                ValueText.color = PlayerDiceTextColor;
                break;
            case RobotIdentity.Enemy:
                DiceBGColor.color = EnemyDiceColor;
                ValueText.color = EnemyDiceTextColor;
                break;
            case RobotIdentity.PlayerAlly:
                DiceBGColor.color = AllyDiceColor;
                ValueText.color = AllyDiceTextColor;
                break;
        }
        ValueText.text = value.ToString();
        OverclockedBackground.SetActive(this.IsOverclocked);
        AdditionalDiceBackground.SetActive(this.IsAdditional);
    }


    public void Set(DiceRollData roll)
    {
        if (GameState.instance != null && GameState.instance.battleData != null && GameState.instance.battleData.CurrentMoveRobotData != null)
            this.Identity = GameState.instance.battleData.CurrentMoveRobotData.Identity;

        value = roll.value;
        this.IsOverclocked = roll.isOverclock;
        this.IsAdditional = roll.isAdditional;


        Redraw();
        gameObject.SetActive(true);

    }

    public void Hide()
    {
        value = 0;
        ValueText.text = value.ToString();
        OverclockedBackground.SetActive(false);
        AdditionalDiceBackground.SetActive(false);
        gameObject.SetActive(false);
    }
}
