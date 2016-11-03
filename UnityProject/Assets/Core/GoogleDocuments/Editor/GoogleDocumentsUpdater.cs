using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class GoogleDocumentsUpdater
{
    private int m_documentConnectionsCount = 0;
    private Dictionary<WWW, GoogleDocumentLink> m_documentConnections = new Dictionary<WWW, GoogleDocumentLink>();

    public event System.Action OnUpdateComplete = null;


    public void UpdateGoogleDocuments(List<GoogleDocumentLink> documents)
    {
        for (int i = documents.Count - 1; i >= 0; --i)
        {
            GoogleDocumentLink document = documents[i];

            string connectionString = document.CreateConnectionString();
            if (string.IsNullOrEmpty(connectionString))
                continue;

            Debug.Log("Connecting to : " + connectionString);
            m_documentConnections.Add(new WWW(connectionString), document);
        }
        m_documentConnectionsCount = documents.Count;
        EditorApplication.update += this.Update;
    }

    public void Stop()
    {
        EditorApplication.update -= this.Update;
        OnUpdateComplete = null;
        EditorUtility.ClearProgressBar();
    }

    public void Update()
    {
        if (m_documentConnections.Count == 0)
        {
            EditorApplication.update -= this.Update;
            if (OnUpdateComplete != null)
            {
                OnUpdateComplete.Invoke();
                OnUpdateComplete = null;
            }
            EditorUtility.ClearProgressBar();
        }
        else
        {
            WWW[] connections = m_documentConnections.Keys.ToArray();
            for (int i = connections.Length - 1; i >= 0; --i)
            {
                EditorUtility.DisplayProgressBar("Google Documents Updater", "Updating ...", (float)Mathf.Abs(m_documentConnectionsCount - m_documentConnections.Count) / (float)m_documentConnectionsCount);

                WWW www = connections[i];
                if (www.isDone == false)
                {
                    Debug.Log("[" + m_documentConnections[www].ID + "] downloading...");
                }
                else
                {
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError("Error for connection : " + m_documentConnections[www].ID);
                        if (www.error != null) Debug.LogError(www.error);
                    }


                    GoogleDocumentLink document = m_documentConnections[www];
                    try
                    {
                        Debug.Log("[" + document.ID + "] Data retrieved : \n" + www.text);
                        m_documentConnections.Remove(www);

                        document.UpdateData(www.text);

                        EditorUtility.ClearProgressBar();
                    }
                    catch (System.Exception ex)
                    {
                        //Debug.LogError("Are you sure you have the document checked out in perforce?");
                        EditorUtility.DisplayDialog
                        (
                            "Error",
                            document.ID + ":" + ex, 
                            "OK"
                        );
                        m_documentConnections.Remove(www);
                    }

                }
            }
        }
    }
}
