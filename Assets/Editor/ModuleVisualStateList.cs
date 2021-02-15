using System;
using UnityEditor;
using UnityEngine;
using static RobotLoadout;

public static class ModuleVisualStateList
{
    public static void Show(SerializedProperty list)
    {
        EditorGUILayout.PropertyField(list);
        EditorGUI.indentLevel += 1;

        EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
        DrawUILine(Color.gray, 1);
        ShowHeader();
        ShowElements(list);

        EditorGUI.indentLevel -= 1;
    }

    private static void ShowHeader()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(nameof(ModuleVisualState.key), GUILayout.MinWidth(50));
        EditorGUILayout.LabelField(nameof(ModuleVisualState.installed), GUILayout.MinWidth(50));
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// in one row
    /// </summary>
    /// <param name="list"></param>
    private static void ShowElements(SerializedProperty list)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            SerializedProperty MyListRef = list.GetArrayElementAtIndex(i);
            SerializedProperty MyEnum = MyListRef.FindPropertyRelative(nameof(ModuleVisualState.key));
            SerializedProperty MyBools = MyListRef.FindPropertyRelative(nameof(ModuleVisualState.installed));

            EditorGUILayout.LabelField(MyEnum.enumValueIndex.GetEnumValueByIndex<ModuleKey>().ToString(), GUILayout.MinWidth(50));
            for (int j = 0; j < MyBools.arraySize; ++j)
            {
                var boolElement = MyBools.GetArrayElementAtIndex(j);
                boolElement.boolValue = EditorGUILayout.Toggle(boolElement.boolValue);
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

}