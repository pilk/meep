using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateCollection : MonoBehaviour
{
    [System.Serializable]
    public class StateEntry
    {
        public string stateName;
        public GameObject stateObject;

        public bool isValid
        {
            get { return !string.IsNullOrEmpty(stateName) && stateObject != null; }
        }
    }

    public string m_defaultState = null;

    public List<StateEntry> m_stateEntries = new List<StateEntry>();
    private StateEntry m_currentState = null;

    public StateEntry currentState
    {
        get { return m_currentState; }
    }

    public List<string> stateNames
    {
        get 
        {
            List<string> names = new List<string>();
            for (int i = m_stateEntries.Count - 1; i >= 0; --i)
            {
                names.Add(m_stateEntries[i].stateName);
            }
            return names; 
        }
    }

#if UNITY_EDITOR
    public string[] GetAvailableStateNames()
    {
        List<string> names = new List<string>(m_stateEntries.Count);
        for (int i = 0; i < m_stateEntries.Count; ++i)
            names.Add(m_stateEntries[i].stateName);
        return names.ToArray();
    }
#endif

    //private void Reset()
    //{
    //    Setup(this.transform);
    //}

    public void SetToDefaultState()
    {
        if (!string.IsNullOrEmpty(m_defaultState))
        {
            this.SetState(m_defaultState, true);
        }
    }

    public void Setup(Transform target)
    {
        target.gameObject.SetActive(true);
        m_stateEntries.Clear();
        m_defaultState = null;
        foreach (Transform t in target)
        {
            if (string.IsNullOrEmpty(m_defaultState))
            {
                m_defaultState = t.gameObject.name.ToLower();
                this.SetState(m_defaultState, true);
            }

            m_stateEntries.Add(new StateEntry()
            {
                stateName = t.gameObject.name.ToLower(),
                stateObject = t.gameObject
            });

        }
    }

    private void OnEnable()
    {
        if (m_currentState == null || m_currentState.isValid == false)
        {
            if (string.IsNullOrEmpty(m_defaultState) && m_stateEntries.Count > 0)
            {
                m_defaultState = m_stateEntries[0].stateName;
            }
            this.SetState(m_defaultState, true);
        }
    }

    public void SetState(string state, bool force = false)
    {
        if (force || m_currentState == null || (m_currentState.stateName.Equals(state) == false))
        {
            m_currentState = null;
            for (int i = m_stateEntries.Count - 1; i >= 0; --i)
            {
                bool isTarget = false;
                isTarget = m_stateEntries[i].stateName.Equals(state);
                if (isTarget)
                {
                    m_currentState = m_stateEntries[i];
                }

                if (m_stateEntries[i].stateObject != null)
                {
                    m_stateEntries[i].stateObject.SetActive(isTarget);
                }
            }

            if (m_currentState == null)
            {
                DebugUtil.LogWarning("Couldn't find state named [" + state + "] on " + gameObject.name);
            }
        }
    }

    public bool HasState(string state)
    {
        for (int i = m_stateEntries.Count - 1; i >= 0; --i)
        {
            if (m_stateEntries[i].stateName.Equals(state,  System.StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    public bool IsEntry(GameObject target)
    {
        for (int i = m_stateEntries.Count - 1; i >= 0; --i)
        {
            if (m_stateEntries[i] != null && m_stateEntries[i].stateObject == target)
                return true;
        }
        return false;
    }

    public StateEntry GetEntryFromObject(GameObject target)
    {
        for (int i = m_stateEntries.Count - 1; i >= 0; --i)
        {
            if (m_stateEntries[i] != null && m_stateEntries[i].stateObject == target)
                return m_stateEntries[i];
        }
        return null;
    }

    public StateEntry GetStateFromIndex(int index)
    {
        if (m_stateEntries.Count > index)
        {
            return m_stateEntries[index];
        }
        return null;
    }

    public int GetCurrentStateIndex()
    {
        if (m_currentState != null)
        {
            for (int i = m_stateEntries.Count - 1; i >= 0; --i)
            {
                if (m_stateEntries[i] != null && m_stateEntries[i] == m_currentState)
                {
                    return i;
                }
            }
        }

        return 0;
    }
}
