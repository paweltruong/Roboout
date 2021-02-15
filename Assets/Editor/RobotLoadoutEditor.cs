using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEditor;

[CustomEditor(typeof(RobotLoadout)), CanEditMultipleObjects]
public class RobotLoadoutEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var roboLoadout = (RobotLoadout)target;
        if (GUILayout.Button("Reload Module Resources for Editor"))
        {
            roboLoadout.ReloadModuleVisualStates();
        }
        EditorGUILayout.HelpBox("Reload buton should be used when new resources are beaing added", MessageType.Info);

        serializedObject.Update();
        ModuleVisualStateList.Show(serializedObject.FindProperty(nameof(RobotLoadout.Modules)));
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(roboLoadout);

            if (!Application.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(roboLoadout.gameObject.scene);
            }
        }
    }
}
