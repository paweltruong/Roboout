using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MARK000", menuName ="Market")]
public class Market : ScriptableObject
{
    //TODO: shop
    public Module[] modulesForSale;
    public Mainframe[] mainframesForSale;
}
