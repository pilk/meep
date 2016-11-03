using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

abstract public class GoogleDocumentLink : ScriptableObject
{
    public string m_documentID;
    public string m_gid = "0";

    public string ID { get { return this.name; } }
    public string documentID { get { return m_documentID; } }
    virtual public string webServiceURL { get { return "https://docs.google.com/spreadsheets/d"; } }
    abstract public string format { get; }

    virtual public void OnCreate()
    {

    }

    virtual public void UpdateData(string text)
    {

    }

    public string CreateConnectionString()
    {
        string connectionString = webServiceURL;
        if (!string.IsNullOrEmpty(documentID)) connectionString += "/" + documentID;
        connectionString += "/" + "export?gid=" + m_gid + "&format=" + format;
        return connectionString;
    }


    protected void P4Checkout(UnityEngine.Object file)
    {
        // TEMP : Need to fix auto check out for mac users
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            // Checkout file from perforce before attempting to write to file
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                startInfo.FileName = "/Applications/p4";
            }
            else
            {
                startInfo.FileName = "C:\\Program Files\\Perforce\\p4.exe";
            }

            string dataPath = Application.dataPath.Remove(Application.dataPath.Length - 6);  //remove /Assets

            startInfo.Arguments = "edit -c default \"" + dataPath + AssetDatabase.GetAssetPath(file) + "\"";
            process.StartInfo = startInfo;
            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                Debug.Log(line);
            }
        }
    }


    protected void LoadMembersViaReflection(object target, Dictionary<string, string> parameters)
    {
        FieldInfo[] fieldInfos = target.GetType().GetFields();
        foreach (FieldInfo f in fieldInfos)
        {
            if (parameters.ContainsKey(f.Name) == false)
                continue;
            f.SetValue(target, parameters[f.Name]);
        }
    }

    protected List<Dictionary<string, string>> GenerateTableFromData(string data)
    {
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();
        if (string.IsNullOrEmpty(data))
            return ret;

        // Seperated into lines
        string[] lines = data.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);

        // Seperated the first line into the category headers
        string[] elements = lines[0].Split(',');
        List<string> headers = new List<string>();
        for (int i = 0; i < elements.Length; ++i)
        {
            headers.Add(elements[i]);
            if (elements[i].Contains("\n"))
            {
                break;
            }
        }

        // Placing the rest of the items in the spreadsheet into a table of objects
        for (int i = 1; i < lines.Length; ++i)
        {
            Dictionary<string, string> dataEntry = new Dictionary<string, string>();
            ret.Add(dataEntry);
            string[] splits = lines[i].Split(',');
            for (int x = 0; x < splits.Length; ++x)
            {
                dataEntry.Add(headers[x], splits[x]);
            }
        }


        return ret;
    }

    protected Dictionary<string, Dictionary<string, string>> GenerateDataByKeyTable(string key, List<Dictionary<string, string>> data)
    {
        Dictionary<string, Dictionary<string, string>> dataByKey = new Dictionary<string, Dictionary<string, string>>();

        foreach (Dictionary<string, string> dataEntry in data)
        {
            if (dataByKey.ContainsKey(dataEntry[key]))
            {
                Debug.LogWarning("Found two items with the same name  : " + dataEntry[key]);
                continue;
            }
            dataByKey.Add(dataEntry[key], dataEntry);
        }

        return dataByKey;
    }
};


// Uses a generic to target a file to output the data to
abstract public class GoogleDocumentLink<T> : GoogleDocumentLink 
    where T : UnityEngine.Object
{
    [Space(20)]
 
    public T outputFile;

    public override void UpdateData(string text)
    {
        base.UpdateData(text);

        if (outputFile == null)
            return;
        P4Checkout(outputFile);
    }
}
