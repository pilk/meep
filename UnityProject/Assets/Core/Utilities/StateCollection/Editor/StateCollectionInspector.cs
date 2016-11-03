using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using Rotorz.ReorderableList;
using LayoutHelper;

[CanEditMultipleObjects]
[CustomEditor(typeof(StateCollection), true)]
public class StateCollectionInspector : Editor
{
    private GameObject transformTarget = null;

    private void OnEnable()
    {
        transformTarget = (target as StateCollection).gameObject;
    }

    public override void OnInspectorGUI()
    {
        StateCollection stateCollection = target as StateCollection;

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        using (new HorizontalLayout())
        {
            if (GUILayout.Button("Setup From Transform Group"))
            {
                if (transformTarget == null)
                {
                    Debug.LogWarning("There was no transform group set");
                }
                else
                {
                    stateCollection.Setup(transformTarget.transform);
                }
            }
            //GUILayout.Label("Transform Group");
            GUI.backgroundColor = Color.grey;
            transformTarget = EditorGUILayout.ObjectField(transformTarget, typeof(GameObject), true) as GameObject;
            GUI.backgroundColor = GUI.color;
        }


        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (stateCollection.currentState == null || stateCollection.currentState.isValid == false)
        {
            stateCollection.SetToDefaultState();
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_defaultState"));

        using (new HorizontalLayout())
        {
            EditorGUILayout.LabelField("Cycle states");

            if (stateCollection.m_stateEntries.Count > 1)
            {
                int currentStateIndex = stateCollection.GetCurrentStateIndex();

                if (GUILayout.Button("<"))
                {
                    --currentStateIndex;
                    if (currentStateIndex < 0)
                        currentStateIndex = stateCollection.m_stateEntries.Count - 1;
                    stateCollection.SetState(stateCollection.GetStateFromIndex(currentStateIndex).stateName);
                }

                if (GUILayout.Button(">"))
                {
                    currentStateIndex = (currentStateIndex + 1) % stateCollection.m_stateEntries.Count;
                    stateCollection.SetState(stateCollection.GetStateFromIndex(currentStateIndex).stateName);
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        ReorderableListGUI.Title("States");
        ReorderableListGUI.ListField(stateCollection.m_stateEntries, delegate(Rect r, StateCollection.StateEntry item)
        {
            if (item == null) item = new StateCollection.StateEntry();

            Rect stateNameArea = new Rect(r);
            stateNameArea.width = r.width / 3;
            item.stateName = EditorGUI.TextField(stateNameArea, item.stateName);

            Rect objectReferenceArea = new Rect(r);
            objectReferenceArea.x = stateNameArea.x + stateNameArea.width + 10;
            objectReferenceArea.width = r.width - objectReferenceArea.x;
            item.stateObject = EditorGUI.ObjectField(objectReferenceArea, item.stateObject, typeof(GameObject), true) as GameObject;

            return item;
        }, delegate { });



        for (int i = 0; i < stateCollection.m_stateEntries.Count; ++i)
        {
            if (stateCollection.m_stateEntries[i] != null && !string.IsNullOrEmpty(stateCollection.m_stateEntries[i].stateName))
            {
                if (stateCollection.m_stateEntries[i] == stateCollection.currentState)
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button(stateCollection.m_stateEntries[i].stateName))
                {
                    stateCollection.SetState(stateCollection.m_stateEntries[i].stateName, true);
                }
                GUI.backgroundColor = GUI.color;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
