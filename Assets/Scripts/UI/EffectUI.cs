using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectUI : MonoBehaviour
{
    [SerializeField] internal Effect type;

    public void Set(object val1, object val2 = null)
    {
        var objectType = this.GetType();
        if (objectType == typeof(EffectUI_SingleValue))
        {
            string value1formatted = string.Empty;
            //formatowania
            switch (type)
            {
                case Effect.Scout:
                    value1formatted = string.Empty;
                    break;
                case Effect.BonusSalvaging:
                case Effect.BonusScavenging:
                    value1formatted = ((float)val1).GetFormattedPercent();
                    break;
                default:
                    value1formatted = val1.ToString();
                    break;
            }
            (this as EffectUI_SingleValue).Set(value1formatted);
        }
        else if (objectType == typeof(EffectUI_DualValue))
        {
            //dual
            string value1formatted = string.Empty;
            string value2formatted = string.Empty;
            //formatowania
            switch (type)
            {
                default:
                    value1formatted = val1.ToString();
                    value2formatted = $"{val2.ToString()}T";
                    break;
            }
            (this as EffectUI_DualValue).SetDual(value1formatted, value2formatted);
        }
        else
        {
            throw new System.Exception($"Effect type not supported: {objectType}");
        }

    }
}
