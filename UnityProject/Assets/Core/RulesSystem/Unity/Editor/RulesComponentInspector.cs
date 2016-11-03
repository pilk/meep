using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using Rotorz.ReorderableList;
using LayoutHelper;

[CustomEditor(typeof(RulesComponent), true)]
public class RulesComponentInspector : Editor
{
    private string m_definitionName = "Definition Name";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RulesComponent rulesComponent = (RulesComponent)target;

        EditorGUILayoutHelpers.ApplicationButton("Apply Definition", () =>
        {
            //CharacterDefinition definition;
            //if (GameSystems.Get<DataCenter.DataCenterManager>().TryGetByName<CharacterDefinition>(rulesComponent.m_definitionName, out definition))
            //{
            //    rulesComponent.Setup(definition);
            //}
        }, null);

        using (new VerticalLayout())
        {
            EditorGUILayout.LabelField("Statuses");
            for (int i = 0, count = rulesComponent.statusInstances.Count; i < count; ++i )
            {
                using (new HorizontalLayout())
                {
                    using (new BackgroundColor(Color.grey))
                    {
                        GUILayout.TextField(rulesComponent.statusInstances[i].statusDefinition.name);
                        GUILayout.TextField(rulesComponent.statusInstances[i].lifeTimer.ToString());
                    }
                }
            }
        }
    }
}
