using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IGameSystemNotification
{
    void OnEvent(string eventName, object eventValue);
};

static public class GameSystems
{
    public delegate void GameSystemEventHandler(string eventName, object eventValue);

    static private readonly Dictionary<string, object> s_systems = new Dictionary<string, object>(16, System.StringComparer.Ordinal);
    static private event GameSystemEventHandler s_notificationEvent = null;

    static public void Register<T>(object target)
    {
        string key = typeof(T).Name;
        if (s_systems.ContainsKey(key))
        {
            DebugUtil.LogError("There is already a type of : " + key + " that exists");
        }
        else
        {
            s_systems.Add(key, target);
        }

        IGameSystemNotification notificationHandler = target as IGameSystemNotification;
        if (notificationHandler != null)
        {
            s_notificationEvent += notificationHandler.OnEvent;
        }
    }

    static public T Get<T>()
    {
        object ret = null;
        s_systems.TryGetValue(typeof(T).Name, out ret);
        return (T)ret;
    }

    static public bool Exists<T>()
    {
        return s_systems.ContainsKey(typeof(T).Name);
    }

    static public void SendEvent(string eventName, object eventValue)
    {
        if (s_notificationEvent != null)
        {
            s_notificationEvent.Invoke(eventName, eventValue);
        }
    }

}
