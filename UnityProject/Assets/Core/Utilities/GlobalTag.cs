using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalTag : MonoBehaviour
{
    static private Dictionary<string, List<System.Action<GameObject>>> m_callbacksByID = new Dictionary<string, List<System.Action<GameObject>>>(System.StringComparer.Ordinal);

    static private Dictionary<string, List<GameObject>> s_objectsByTagID = new Dictionary<string, List<GameObject>>(System.StringComparer.Ordinal);
    static bool TryGet(string tagID, out List<GameObject> objects)
    {
        return s_objectsByTagID.TryGetValue(tagID, out  objects);
    }



    public List<string> m_tags = new List<string>(4);






    static public void RunActionOnObject(string tagID, System.Action<GameObject> action)    
    {
        if (s_objectsByTagID.ContainsKey(tagID))
        {
            for (int i = s_objectsByTagID[tagID].Count - 1; i >= 0; --i)
            {
                action(s_objectsByTagID[tagID][i]);
            }
        }
    }

    static public void RunActionOnComponent<T>(string tagID, System.Action<T> action) where T : MonoBehaviour
    {
        if (s_objectsByTagID.ContainsKey(tagID))
        {
            for (int i = s_objectsByTagID[tagID].Count - 1; i >= 0; --i)
            {
                action(s_objectsByTagID[tagID][i].GetComponent<T>());
            }
        }
    }

    static public void RegisterActionForObject(string tagID, System.Action<GameObject> action)
    {
        if (m_callbacksByID.ContainsKey(tagID) == false)
            m_callbacksByID.Add(tagID, new List<System.Action<GameObject>>(4));
        m_callbacksByID[tagID].Add(action);
    }

    static public void UnregisterActionForObject(string tagID, System.Action<GameObject> action)
    {
        m_callbacksByID[tagID].Remove(action);
    }


    private void Awake()
    {
        Register();
    }

    private void OnDestroy()
    {
        Unregister();
    }

    private void Register()
    {
        for (int i = m_tags.Count - 1; i >= 0; --i)
        {
            string tag = m_tags[i];
            if (s_objectsByTagID.ContainsKey(tag) == false)
                s_objectsByTagID.Add(tag, new List<GameObject>(8));
            s_objectsByTagID[tag].Add(this.gameObject);
            if (m_callbacksByID.ContainsKey(tag))
            {
                for (int x = m_callbacksByID[tag].Count - 1; x >= 0; --x)
                {
                    m_callbacksByID[tag][x](this.gameObject);
                }
            }
        }
    }

    private void Unregister()
    {
        for (int i = m_tags.Count - 1; i >= 0; --i)
        {
            s_objectsByTagID[m_tags[i]].Remove(this.gameObject);
        }
    }

    public void SetTags(List<string> tags)
    {
        Unregister();
        m_tags = tags;
        Register();
    }
}
