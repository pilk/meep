using UnityEngine;
using System.Collections;
using System;

public class DunkleGameLoader : GameLoader
{
    protected override IEnumerator LoadingSequence()
    {
        Transform gameSystems = (new GameObject("GameSystem")).transform;
        DontDestroyOnLoad(gameSystems.gameObject);


        DataCenter.DataCenterManager dataCenterManager = new DataCenter.DunkleDataCenterManager();
        GameSystems.Register<DataCenter.DataCenterManager>(dataCenterManager);
        dataCenterManager.Initialize("files", "");
        yield return null;


        Rules.RulesSystem rulesSystem = new DunkleRulesSystem();
        GameSystems.Register<Rules.RulesSystem>(rulesSystem);
        GameSystems.Register<DunkleRulesSystem>(rulesSystem);
        rulesSystem.Initialize();
        yield return null;

        EventCenter eventCenter = new EventCenter();
        GameSystems.Register<EventCenter>(eventCenter);
        yield return null;


        SavedData savedData = (new GameObject("SavedData")).AddComponent<SavedData>();
        GameSystems.Register<SavedData>(savedData);
        savedData.transform.parent = gameSystems;
        yield return null;



        SessionData sessionData = (new GameObject("SessionData")).AddComponent<SessionData>();
        GameSystems.Register<SessionData>(sessionData);
        sessionData.transform.parent = gameSystems;
        yield return null;
    }
}