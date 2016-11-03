using UnityEngine;
using UnityEditor;
using System.Collections;

public class DebugAssetCreator 
{
    [MenuItem("Assets/Create/Debug/Create Debug Util")]
    public static void CreateDebugUtil()
    {
        ScriptableObjectUtility.CreateAsset<DebugUtil>();
    }
}
