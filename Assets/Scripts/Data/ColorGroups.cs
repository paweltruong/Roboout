
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class ColorGroups
{
    public string groupName;
    public Color color = new Color32(255,255,255,255);
    public SpriteRenderer[] parts;
}