using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CameraTag), true)]
public class CameraTagInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        EditorGUILayoutHelpers.ApplicationButton("Transition to Camera",
            () => { CameraController.Instance.SetCamera((CameraTag)target); },
            null
        );
    }
}
