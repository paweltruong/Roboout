using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Utils
{
    public static string ToShortString(this SlotType slotType)
    {
        switch (slotType)
        {
            case SlotType.Arm:
                return "Arm";
            case SlotType.Head:
                return "Head";
            case SlotType.Leg:
                return "Leg";
            case SlotType.Reactor:
                return "Reac";
            case SlotType.Utility:
                return "Util";
            case SlotType.Storage:
                return "Store";
            default:
                return "?";
        }
    }

    public static string ToDebugString(this Mainframe mainframe)
    {
        return GetDebugString(mainframe);
    }

    static string GetDebugString(object o)
    {
        StringBuilder sb = new StringBuilder();
        var props = o.GetType().GetProperties();
        foreach (var p in props)
        {
            sb.AppendLine($"{p.Name}: {p.GetValue(o).ToString()}");
        }
        return sb.ToString();
    }

    public static int GetEnumElementIndexByName<TEnum>(this TEnum enumValue)
    {
        var names = Enum.GetNames(typeof(TEnum));
        for (int i = 0; i < names.Length; ++i)
            if (enumValue.ToString() == names[i])
                return i;
        return -1;
    }
    public static TEnum GetEnumValueByName<TEnum>(this string enumName)
    {
        var values = Enum.GetValues(typeof(TEnum));
        var names = Enum.GetNames(typeof(TEnum));
        for (int i = 0; i < names.Length; ++i)
            if (enumName.ToLower() == names[i].ToLower())
                return (TEnum)values.GetValue(i);
        Debug.LogError($"enumvalue for {enumName} not found");
        return (TEnum)values.GetValue(0);
    }


    public static TEnum GetEnumValueByIndex<TEnum>(this int index)
    {
        var values = Enum.GetValues(typeof(TEnum));
        if (index >= 0 && index < values.Length)
            return (TEnum)values.GetValue(index);
        else
            Debug.LogError("Invalid index");
        return (TEnum)values.GetValue(0);
    }

    public static Color ToColor(this ModuleQuality moduleQuality)
    {
        switch (moduleQuality)
        {
            case ModuleQuality.Rare:
                return Colors.ItemQualityColor_Rare;
            case ModuleQuality.Set:
                return Colors.ItemQualityColor_Set;
            default:
                return Colors.ItemQualityColor_Common;
        }
    }


    public static Color RankToColor(this int rank)
    {
        switch (rank)
        {
            case 2:
                return Colors.Rank_2;
            case 1:
                return Colors.Rank_1;
            default:
                return Colors.Rank_3;
        }
    }

    public static string ToProcessorOutputString(this DiceRoll dr)
    {
        List<int> values = new List<int>();
        if (dr.HasFlag(DiceRoll.D1))
            values.Add(1);
        if (dr.HasFlag(DiceRoll.D2))
            values.Add(2);
        if (dr.HasFlag(DiceRoll.D3))
            values.Add(3);
        if (dr.HasFlag(DiceRoll.D4))
            values.Add(4);
        if (dr.HasFlag(DiceRoll.D5))
            values.Add(5);
        if (dr.HasFlag(DiceRoll.D6))
            values.Add(6);


        return string.Join("/", values); ;
    }

    public static bool RollValueMatchedDiceRoll(this int value, DiceRoll roll)
    {
        if (roll.HasFlag(DiceRoll.D1) && value == 1)
            return true;
        if (roll.HasFlag(DiceRoll.D2) && value == 2)
            return true;
        if (roll.HasFlag(DiceRoll.D3) && value == 3)
            return true;
        if (roll.HasFlag(DiceRoll.D4) && value == 4)
            return true;
        if (roll.HasFlag(DiceRoll.D5) && value == 5)
            return true;
        if (roll.HasFlag(DiceRoll.D6) && value == 6)
            return true;
        return false;
    }

    /// <summary>
    /// 0.1f => 10%
    /// </summary>
    /// <param name="value">1f == 100%</param>
    /// <returns></returns>
    public static string GetFormattedPercent(this float value)
    {
        return $"{Mathf.FloorToInt((float)value * 100)}%";
    }

    public static string ToMapEncounterName(this EncounterUI encounterUI)
    {
        string value = "Unknown";
        if (encounterUI.isAvailable && encounterUI.isScouted)
        {
            value = encounterUI.type.ToString();
        }
        return value;
    }

    public static IEnumerable<KeyValuePair<int, TObject>> ToIndexed<TObject>(this IEnumerable<TObject> collection,
        Func<KeyValuePair<int, TObject>, bool> predicate = null)
    {
        if (predicate == null)
            return collection.Select((o, i) => new KeyValuePair<int, TObject>(i, o));
        return collection.Select((o, i) => new KeyValuePair<int, TObject>(i, o)).
               Where(predicate);
    }
    public static IEnumerable<KeyValuePair<int, TObject>> ToIndexed<TObject>(this IEnumerable<TObject> collection,
       Func<KeyValuePair<int, TObject>, bool> predicate, IEnumerable<int> excludedIndices)
    {
        return collection.Select((o, i) => new KeyValuePair<int, TObject>(i, o)).
               Where(predicate).Where(kvp => !excludedIndices.Contains(kvp.Key));
    }

    public static EncounterDifficulty GetRandomDifficulty(this KeyValuePair<int, EncounterBlueprint> indexedEB)
    {
        return (EncounterDifficulty)UnityEngine.Random.Range((int)indexedEB.Value.minDifficulty, (int)indexedEB.Value.maxDifficulty + 1);
    }

    public static AudioClip GetRandom(this AudioClip[] clipPool)
    {
        if (clipPool != null && clipPool.Length > 0)
        {
            return clipPool[UnityEngine.Random.Range(0, clipPool.Length)];
        }
        return null;
    }
}
