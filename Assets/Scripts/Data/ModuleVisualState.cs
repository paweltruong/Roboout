

[System.Serializable]
public struct ModuleVisualState
{
    public ModuleKey key;
    /// <summary>
    /// can have 2 heads f.e. 0,1  on which sortingLayer this will be displayed is set on robotPrefab
    /// </summary>
    public bool[] installed;
}