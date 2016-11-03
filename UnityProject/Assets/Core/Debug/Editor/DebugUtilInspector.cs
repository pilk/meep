using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using LayoutHelper;

[CustomEditor(typeof(DebugUtil), true)]
public class DebugUtilInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ReorderableListGUI.Title("Debug Color Prefs");
        ReorderableListGUI.ListField(((DebugUtil)target).m_debugLogColorPrefs, DisplayDebugColorPref, () => { });
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(this);
    }

    private DebugUtil.DebugLogColorPref DisplayDebugColorPref(Rect p, DebugUtil.DebugLogColorPref item)
    {
        const float COLOR_WIDGET_WIDTH = 50.0f;

        if (item == null) item = new DebugUtil.DebugLogColorPref();

        item.m_color = EditorGUI.ColorField(new Rect(p.x, p.y, COLOR_WIDGET_WIDTH, p.height), item.m_color);     
        item.m_flag = (DebugFlags)EditorGUI.EnumMaskField( new Rect( p.x + COLOR_WIDGET_WIDTH, p.y, p.width - COLOR_WIDGET_WIDTH, p.height), item.m_flag);

        return item;
    }
}
