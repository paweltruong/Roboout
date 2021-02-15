using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine;

//TODO:refactor prop & field names
[System.Serializable]
public class PaintJobGroups
{
    public SpriteRenderer[] BodyMain;
    public SpriteRenderer[] BodySecondary;
    public SpriteRenderer[] Detail1;
    public SpriteRenderer[] Detail2;
    public SpriteRenderer[] Detail3;
    public SpriteRenderer[] Plugs;
}

[System.Serializable]
public class PaintJobSpecificParts
{
    public PaintJob paintJobKey;
    public GameObject[] AdditionalGameObjects;
}


[System.Serializable]
internal class PaintJobElementSetting
{
    [SerializeField]
    PaintJobElement m_element;
    [SerializeField]
    Color m_Color;

    public string Name
    {
        get { return m_element.ToString(); }
    }

    public PaintJobElement element
    {
        get { return m_element; }
        set
        {
            m_element = value;
        }
    }
    public Color color { get { return m_Color; } set { m_Color = value; } }
}

[System.Serializable]
internal class PaintJobSettings //: INameHash
{
    [SerializeField]
    PaintJob m_Key;

    public Color BodyMain = Color.white;
    public Color BodySecondary = Color.white;
    public Color Detail1 = Color.white;
    public Color Detail2 = Color.white;
    public Color Detail3 = Color.white;
    public Color Plugs = Color.white;
    
    public PaintJob Key
    {
        get { return m_Key; }
        set
        {
            m_Key = value;
        }
    }

    public string Name
    {
        get { return m_Key.ToString(); }
    }

    public List<PaintJobElementSetting> elementList
    {
        get
        {
            var list = new List<PaintJobElementSetting>();
            list.Add(new PaintJobElementSetting { element = PaintJobElement.BodyMain, color = BodyMain });
            list.Add(new PaintJobElementSetting { element = PaintJobElement.BodySecondary, color = BodySecondary });
            list.Add(new PaintJobElementSetting { element = PaintJobElement.Detail1, color = Detail1 });
            list.Add(new PaintJobElementSetting { element = PaintJobElement.Detail2, color = Detail2 });
            list.Add(new PaintJobElementSetting { element = PaintJobElement.Detail3, color = Detail3 });
            list.Add(new PaintJobElementSetting { element = PaintJobElement.Plugs, color = Plugs });
            return list;
        }
    }


    internal void ValidateLabels()
    {
        //TODO:?
    }
}

[CreateAssetMenu(order = 351, menuName = "PaintJob Library Asset")]
public class PaintJobLibrary : ScriptableObject
{
    public PaintJobLibrary()
    {
        m_paintjobs = new List<PaintJobSettings>();
        foreach (var enumValue in System.Enum.GetValues(typeof(PaintJob)))
        {
            var paintJobSetting = new PaintJobSettings()
            {
                Key = (PaintJob)enumValue,
            };

            m_paintjobs.Add(paintJobSetting);
        }
    }

    [SerializeField]
    private List<PaintJobSettings> m_paintjobs = new List<PaintJobSettings>();

    internal List<PaintJobSettings> Paintjobs
    {
        get
        {
            return m_paintjobs;
        }
        set
        {
            m_paintjobs = value;
            //ValidatePaintjobs();
        }
    }

    public Color GetColor(PaintJob paintJob, PaintJobElement element)
    {
        var paintJobSetting = m_paintjobs.FirstOrDefault(x => x.Key == paintJob);
        if (paintJobSetting != null)
        {
            var spritelabel = paintJobSetting.elementList.FirstOrDefault(x => x.element == element);
            if (spritelabel != null)
            {
                return spritelabel.color;
            }
        }

        return Color.white;
    }
}

