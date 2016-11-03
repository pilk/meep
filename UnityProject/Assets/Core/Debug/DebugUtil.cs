using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class DebugUtil : ScriptableObject
{
    public const string DEBUG_BUILD_DEFINE_SYMBOL = "DEBUG_BUILD";

    [System.Serializable]
    public class DebugLogColorPref
    {
        [SerializeField]
        [EnumFlagsAttribute]
        public DebugFlags m_flag;

        public Color m_color;
    };

    [SerializeField]
    [EnumFlagsAttribute]
    public DebugFlags m_currentFlags = DebugFlags.Default;
    public List<DebugLogColorPref> m_debugLogColorPrefs = new List<DebugLogColorPref>();



    static private DebugUtil s_instance;
    static private DebugUtil instance
    {
        get {
            if (s_instance == null)
            {
                s_instance = Resources.Load<DebugUtil>("DebugUtil");
            }
            return s_instance;
        }
    }


    [Conditional(DEBUG_BUILD_DEFINE_SYMBOL)]
    public static void Log(string text)
    {
        Log(DebugFlags.Default, text);
    }

    [Conditional(DEBUG_BUILD_DEFINE_SYMBOL)]
    public static void Log(DebugFlags filter, string text)
    {
        if ((int)(DebugUtil.instance.m_currentFlags & filter) != 0)
        {
            UnityEngine.Debug.Log(FormatText(filter, text));
        }
    }

    //[Conditional(DEBUG_BUILD_DEFINE_SYMBOL)]
    public static void LogError(string text)
    {
        LogError(DebugFlags.Default, text);
    }

    //[Conditional(DEBUG_BUILD_DEFINE_SYMBOL)]
    public static void LogError(DebugFlags filter, string text)
    {
        if ((int)(DebugUtil.instance.m_currentFlags & filter) != 0)
        {
            UnityEngine.Debug.LogError(FormatText(filter, text));
        }
    }

    //[Conditional(DEBUG_BUILD_DEFINE_SYMBOL)]
    public static void LogWarning(string text)
    {
        LogWarning(DebugFlags.Default, text);
    }

    //[Conditional(DEBUG_BUILD_DEFINE_SYMBOL)]
    public static void LogWarning(DebugFlags filter, string text)
    {
        if ((int)(DebugUtil.instance.m_currentFlags & filter) != 0)
        {
            UnityEngine.Debug.LogWarning(FormatText(filter, text));
        }
    }

    public static string FormatText(DebugFlags filter, string text)
    {
#if UNITY_EDITOR
        for (int i = instance.m_debugLogColorPrefs.Count - 1; i >= 0; --i)
        {
            if (instance.m_debugLogColorPrefs[i].m_flag == filter)
            {
                string colorHex = ColorToHex(instance.m_debugLogColorPrefs[i].m_color);
                return string.Format("<color={0}>[{1}] :</color> {2}", colorHex, filter.ToString(), text);
            }
        }
#endif

        return string.Format("[{0}] : {1}", filter.ToString(), text);
    }

    // Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
    static string ColorToHex(Color32 color)
    {
        return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
    }

    static Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }

    static public void DrawLines(Vector3[] positions, Color color)
    {
        for (int i = 1; i < positions.Length; ++i)
        {
            Vector3 p1 = positions[i - 1];
            Vector3 p2 = positions[i];
            UnityEngine.Debug.DrawLine(p1, p2, color);
        }
    }


    static public void DrawLines(List<Vector3> positions, Color color)
    {
        for (int i = 1; i < positions.Count; ++i)
        {
            Vector3 p1 = positions[i - 1];
            Vector3 p2 = positions[i];
            UnityEngine.Debug.DrawLine(p1, p2, color);
        }
    }
}