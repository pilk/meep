using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GoogleDocumentSpreadSheet : GoogleDocumentLink<TextAsset>
{
    public override string format { get { return "csv"; } }

    override public void OnCreate()
    {
        
    }

#if UNITY_EDITOR
    public override void UpdateData(string text)
    {
        base.UpdateData(text);
        string filePath = Application.dataPath + "/Resources/" + this.name + "." + this.format;
        if (this.outputFile != null)
        {
            filePath = AssetDatabase.GetAssetPath(this.outputFile.GetInstanceID());
        }

        Debug.Log("[" + this.name + "] Saving to file : " + filePath);
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            byte[] byData = Encoding.UTF8.GetBytes(text);
            stream.Write(byData, 0, byData.Length);
            stream.Close();
        }

        if (this.outputFile == null)
        {
            this.outputFile = Resources.Load(this.name) as TextAsset;
            EditorUtility.SetDirty(this);
        }
        AssetDatabase.Refresh();
    }
#endif
}
