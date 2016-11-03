using UnityEditor;
using UnityEngine;
using System.Collections;

namespace LayoutHelper
{
	public class HorizontalLayout : System.IDisposable
	{
		public HorizontalLayout()
		{
			GUILayout.BeginHorizontal();
		}
		public void Dispose()
		{
			GUILayout.EndHorizontal();
		}
	}
	
	public class VerticalLayout : System.IDisposable
	{
		public VerticalLayout()
		{
			GUILayout.BeginVertical();
		}
		public void Dispose()
		{
			GUILayout.EndVertical();
		}
	}
	public class BackgroundColor : System.IDisposable
	{
		Color prevColor = Color.white;

		public BackgroundColor(Color bgColor)
		{
			prevColor = GUI.backgroundColor;
			GUI.backgroundColor = bgColor;
		}
		public void Dispose()
		{
			GUI.backgroundColor = prevColor;
		}
	}

    public class ApplicationPlaying : System.IDisposable
    {
        public static Color disabledColor = Color.grey;
        Color prevColor = Color.white;
        public ApplicationPlaying()
        {
            prevColor = GUI.backgroundColor;
            if (Application.isPlaying == false)
            {
                GUI.backgroundColor = disabledColor;
            }
        }

        public void Dispose()
        {
            GUI.backgroundColor = prevColor;
        }
    }
}
