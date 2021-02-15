using UnityEditor;
using UnityEngine;

public static class EditorExtensions
{
    [MenuItem("Assets/Fix Module Key by name")]
    private static void FixModuleKeyByName()
    {
        Module[] modules = Resources.LoadAll<Module>("Modules");

        foreach (var module in modules)
        {
            var keyFromName = GetKeyFromAssetName(module);
            if (keyFromName != ModuleKey.None)
            {
                //Update only when needed
                if (module.Key != keyFromName)
                {
                    module.Key = keyFromName;
                    EditorUtility.SetDirty(module);
                    Debug.Log($"Module '{module.name}' fixed with Key '{keyFromName}'");
                }
            }
            else
            {
                Debug.LogWarning($"Module name '{module.name}' does not match any known ModuleKey");
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static ModuleKey GetKeyFromAssetName(Module module)
    {
        string separator = "_";
        string keyName = string.Empty;

        //we need second separator f.e ARM_001_XX
        int firstIndex = module.name.IndexOf(separator);
        if (firstIndex > -1)
        {
            keyName = module.name.Substring(0, firstIndex + 1);
            //module asset has following format XXX_Y
            if (module.name.Length > firstIndex + 1)
            {
                string rest = module.name.Substring(firstIndex + 1);

                int index = rest.IndexOf(separator);
                //check if format is XXX_YY_ZZ
                if (index > -1)
                {
                    keyName = keyName + rest.Substring(0, index);
                }
                else
                {
                    //when format is XXX_YY
                    keyName = keyName + rest;
                }
                return keyName.GetEnumValueByName<ModuleKey>();
            }
        }
        Debug.LogError($"Could not parse ModuleKey from {module.name}");
        return ModuleKey.None;
    }
}