using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CameraController), true)]
public class CameraControllerInspector : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        EditorGUILayoutHelpers.ApplicationButton("Transition to Camera",
            ((CameraController)target).TransitionToTargetCamera,
            null
        );
    }
}
