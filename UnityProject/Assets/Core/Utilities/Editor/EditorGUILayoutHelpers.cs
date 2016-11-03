using UnityEngine;
using UnityEditor;
using System.Collections;

public static class EditorGUILayoutHelpers 
{
    public static void ApplicationButton(string text, System.Action onSuccessCallback, System.Action onFailureCallback)
    {
        bool buttonPressed = false;
        using (new LayoutHelper.ApplicationPlaying())
        {
            buttonPressed = GUILayout.Button(text);
        }

        if (buttonPressed)
        {
            if (Application.isPlaying)
            {
                if (onSuccessCallback != null)
                    onSuccessCallback.Invoke();
            }
            else
            {
                if (onFailureCallback != null )
                    onFailureCallback.Invoke();
                EditorUtility.DisplayDialog
                (
                    "Error",
                    "This is only allowed in Play Mode!",
                    "OK"
                );
            }
        }
    }
}
