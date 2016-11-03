using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public partial class GoogleDocumentsEditorMenu
{
    public const string MENU_ITEM = "Google Documents/";
    public const string CREATE_MENU_ITEM = "Assets/Create/Google Documents/";

    protected static GoogleDocumentsUpdater updaterInProgress = null;

    static protected List<T> FindDocumentsOfType<T>() where T : GoogleDocumentLink
    {
        List<T> googleDocuments = new List<T>();
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T), null);
        foreach (string guid in guids)
        {
            T spreadsheet = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as T;
            if (spreadsheet != null)
            {
                googleDocuments.Add(spreadsheet);
            }
        }
        return googleDocuments;
    }


    [MenuItem(MENU_ITEM + "Update Selected Documents")]
    static public void UpdateSelectedDocuments()
    {
        if (updaterInProgress != null)
        {
            Debug.LogWarning("Updater already in progress");
            return;
        }

        List<GoogleDocumentLink> googleDocuments = new List<GoogleDocumentLink>();
        Object[] selections = null;

        selections = Selection.GetFiltered(typeof(GoogleDocumentLink), SelectionMode.DeepAssets);
        foreach (GoogleDocumentLink document in selections)
        {
            googleDocuments.Add(document);
        }

        updaterInProgress = new GoogleDocumentsUpdater();
        updaterInProgress.OnUpdateComplete += delegate { updaterInProgress = null; };
        updaterInProgress.UpdateGoogleDocuments(new List<GoogleDocumentLink>(googleDocuments));
    }

    [MenuItem(MENU_ITEM + "Cancel Update")]
    static public void CancelUpdate()
    {
        if (updaterInProgress != null)
        {
            updaterInProgress.Stop();
            updaterInProgress = null;
        }
    }

    [MenuItem(CREATE_MENU_ITEM + "Link/Google Spread Sheet To CSV Link")]
    static public void CreateGoogleSpreadSheetLink()
    {
        ScriptableObjectUtility.CreateAsset<GoogleDocumentSpreadSheet>().OnCreate();
    }
}
