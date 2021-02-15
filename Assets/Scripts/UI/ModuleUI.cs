using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main class for presenting module cards (not module real part in robot for this look ModuleVisualization)
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ModuleUI : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] ModuleQuality Quality;
    [SerializeField] ModuleUsages Usage;
    [SerializeField] ModuleKey Key;
    [SerializeField] int SlotIndex;
    [SerializeField] int HandIndex;
    [Header("Essential")]
    [SerializeField] TextMeshProUGUI moduleName;
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] Image ModuleBorder;
    [SerializeField] Image ModuleBGMain;
    [SerializeField] Image ModuleBGEffects;
    [SerializeField] EffectUI[] effects;
    [SerializeField] ProcessorOutputUI processorOutput;
    [SerializeField] ActionCostUI actionCost;
    [SerializeField] Image PreviewImage;
    [SerializeField] TextMeshProUGUI ShortcutText;
    [SerializeField] GameObject ShortcutGO;

    float maxWidth = 120;
    float maxHeight = 80;

    public System.Guid Id;
    /// <summary>
    /// Owner of a module
    /// </summary>
    public System.Guid RobotId;
    public string ModuleName => moduleName.text;

    public bool WasDraw { get; private set; }

    void Start()
    {
        if (moduleName == null)
            Debug.LogError("modulename not found");
        if (rank == null)
            Debug.LogError("Rank not found");

        if (ModuleBorder == null || ModuleBGMain == null || ModuleBGEffects == null)
            Debug.LogError("BG not found");
    }
    void HideAllEffects()
    {
        if (effects != null)
            for (int i = 0; i < effects.Length; ++i)
                effects[i].gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor)
        {
            Redraw();
        }
    }

    private void Redraw()
    {
        SetUsage();
        SetQuality();
        SetPreview(Key);
        SetShortcut();
    }

    private void SetShortcut()
    {
        //not in loadout screen
        if (WasDraw
            && this.HandIndex >= 0
            && GameState.instance != null
            && GameState.instance.battleData != null
            && GameState.instance.battleData.CurrentMoveRobotData.Identity == RobotIdentity.Player
            )
        {

            switch (this.HandIndex)
            {
                case 0:
                    ShortcutText.text = "Q";
                    break;
                case 1:
                    ShortcutText.text = "W";
                    break;
                case 2:
                    ShortcutText.text = "E";
                    break;
                case 3:
                    ShortcutText.text = "R";
                    break;
                default:
                    ShortcutText.text = "?";
                    break;
            }
            ShortcutGO.SetActive(true);
        }
        else
            ShortcutGO.SetActive(false);
    }

    void SetUsage()
    {
        if (Usage.HasFlag(ModuleUsages.Active))
        {
            ModuleBorder.color = Colors.ModuleActive;
        }
        else
        {
            ModuleBorder.color = Colors.ModulePassive;
        }
    }
    void SetQuality()
    {
        ModuleBGMain.color = Quality.ToColor();
    }

    public void Set(ModuleInstance module, bool wasDraw, System.Guid robotId)
    {
        this.WasDraw = wasDraw;
        this.Id = module.Id;
        this.SlotIndex = module.SlotIndex;
        if (wasDraw)
            this.HandIndex = GetModuleIndexAtHand();
        HideAllEffects();

        moduleName.text = module.ModuleName;
        rank.text = module.Rank.ToString();
        actionCost.Set(module.PowerCost);
        processorOutput.Set(module.DiceToActivate);

        Key = module.Key;

        ShowEffects(module);

        Usage = module.Usage;
        Quality = module.Quality;
        Redraw();
    }

    int GetModuleIndexAtHand()
    {
        if (GameState.instance != null && GameState.instance.battleData != null)
        {
            for (int i = 0; i < GameState.instance.battleData.ModulesAtHand.Count(); ++i)
                if (GameState.instance.battleData.ModulesAtHand.ElementAt(i).Id == this.Id)
                    return i;
        }
        return -1;
    }

    /// <summary>
    /// set module image preview(black&white) on module UI panel
    /// </summary>
    /// <param name="moduleKey"></param>
    private void SetPreview(ModuleKey moduleKey)
    {
        if (GameState.instance != null && GameState.instance.assetRepo != null)
        {
            var previewSprite = GameState.instance.assetRepo.PreviewSprites.FirstOrDefault(ps => ps.moduleKey == moduleKey);
            if (previewSprite != null)
            {
                PreviewImage.sprite = previewSprite.Sprite;
                float scale = 1;
                float widthScale = maxWidth / previewSprite.Sprite.rect.width;
                float heightScale = maxHeight / previewSprite.Sprite.rect.height;
                if (previewSprite.Sprite.rect.width > maxWidth && previewSprite.Sprite.rect.height > maxHeight)
                {
                    //both dimension excedes max size, have to scale according to smaller dimension so wverything fits in
                    scale = Mathf.Min(widthScale, heightScale);
                }
                else if (previewSprite.Sprite.rect.width > maxWidth && previewSprite.Sprite.rect.height <= maxHeight)
                {
                    //only width exceeds max size, scale based on with scale factor
                    scale = widthScale;
                }
                else if (previewSprite.Sprite.rect.width <= maxWidth && previewSprite.Sprite.rect.height > maxHeight)
                {
                    //only height exceeds max size, scale based on hight scale factor
                    scale = heightScale;
                }
                float scaledWidth = previewSprite.Sprite.rect.width * scale;
                float scaledHeight = previewSprite.Sprite.rect.height * scale;
                PreviewImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledWidth);
                PreviewImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaledHeight);
            }
        }
    }

    /// <summary>
    /// show effect icon and value in effect panel on module UI
    /// </summary>
    /// <param name="module"></param>
    void ShowEffects(ModuleInstance module)
    {
        if (effects != null)
            for (int i = 0; i < effects.Length; ++i)
            {
                TryEnableEffect(effects[i], module);
            }
    }

    /// <summary>
    /// enable disable effect UI based on module configuration
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="module"></param>
    void TryEnableEffect(EffectUI effect, ModuleInstance module)
    {
        switch (effect.type)
        {
            case Effect.DamageThermal:
                if (module.DamageThermal > 0)
                {
                    effect.Set(module.DamageThermal);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.DamageKinetic:
                if (module.DamageKinetic > 0)
                {
                    effect.Set(module.DamageKinetic);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.DisableModuleForXTurns:
                if (module.DisableRandomForXTurns > 0)
                {
                    effect.Set(1/*module count, only 1 supported at first*/, module.DisableRandomForXTurns);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.PowerSubstractEachTurnOverXTurns:
                if (module.ReducePowerForXTurns > 0)
                {
                    effect.Set(1, module.ReducePowerForXTurns);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.Block:
                if (module.BlockStrength > 0)
                {
                    effect.Set(module.BlockStrength);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.BonusMaxPower:
                if (module.BonusMaxHp > 0)
                {
                    effect.Set(module.BonusMaxHp);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.ThermalShieldForXTurns:
                if (module.BonusShield > 0)
                {
                    effect.Set(module.BonusShield);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.PowerRegen:
                if (module.HpRegen > 0)
                {
                    effect.Set(module.HpRegen);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.BonusScavenging:
                if (module.BonusScavenging > 0)
                {
                    effect.Set(module.BonusScavenging);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.BonusSalvaging:
                if (module.BonusSalvaging > 0)
                {
                    effect.Set(module.BonusSalvaging);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.BonusDamageThermal:
                if (module.BonusDamageThermal > 0)
                {
                    effect.Set(module.BonusDamageThermal);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.BonusDamageKinetic:
                if (module.BonusDamageKinetic > 0)
                {
                    effect.Set(module.BonusDamageKinetic);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.Scout:
                if (module.RevealDungeonInfo)
                {
                    effect.Set(module.RevealDungeonInfo);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.RegenPowerForKill:
                if (module.RegenPowerAfterKill > 0)
                {
                    effect.Set(module.RegenPowerAfterKill);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.StealPower:
                if (module.HpForEnemy > 0)
                {
                    effect.Set(module.HpForEnemy);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.Armor:
                if (module.BonusArmor > 0)
                {
                    effect.Set(module.BonusArmor);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            case Effect.Overclock:
                if (module.ProcessorOverclock > 0)
                {
                    effect.Set(module.ProcessorOverclock);
                    effect.gameObject.SetActive(true);
                }
                else
                    effect.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
}
