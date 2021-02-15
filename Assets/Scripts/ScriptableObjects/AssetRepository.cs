using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetRepository", menuName ="Asset Repo")]
public class AssetRepository : ScriptableObject
{
    [SerializeField]
    public ModuleSprite[] PreviewSprites;


    [Header("Default Player Settings")]
    [SerializeField]
    public Mainframe defaultMainframe;
    [SerializeField]
    public Module[] defaultReactors;
    [SerializeField]
    public Module[] defaultHeads;
    [SerializeField]
    public Module[] defaultArms;
    [SerializeField]
    public Module[] defaultLegs;
}
