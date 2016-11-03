using System.Collections;
using System.Collections.Generic;

namespace Rules
{
    sealed public class RulesMixin
    {
        public List<StatusInstance> m_statusInstanceList = new List<StatusInstance>(16);
        private List<StatusInstance> m_cachedStatusRemovalList = new List<StatusInstance>(4);

        public void AddStatus(DataCenter.StatusDefinition statusDef, object source, object target)
        {
            StatusInstance newStatusInstance = new StatusInstance(statusDef, source, target);

            // Are we able to add the new status?
            Dictionary<string, int> statusGroupStackCounts = new Dictionary<string, int>(System.StringComparer.Ordinal);
            StatusInstance[] statusInstances = m_statusInstanceList.ToArray();
            for (int i = statusInstances.Length - 1; i >= 0; --i)
            {
                StatusInstance other = statusInstances[i];
                for (int j = statusDef.statusGroupNameList.Count - 1; j >= 0; --j)
                {
                    string statusGroupName = statusDef.statusGroupNameList[j];


                    // Find a shared status type
                    if (other.statusDefinition.statusGroupNameList.Contains(statusGroupName))
                    {
                        DataCenter.StatusGroupDefinition statusGroup;
                        DataCenter.DataCenterManager.Instance.TryGetByName<DataCenter.StatusGroupDefinition>(statusGroupName, out statusGroup);

                        if (statusGroupStackCounts.ContainsKey(statusGroupName) == false)
                            statusGroupStackCounts.Add(statusGroupName, 1);
                        else
                            statusGroupStackCounts[statusGroupName]++;

                        // Check if we have reached the stack limit
                        if (statusGroupStackCounts[statusGroupName] < statusGroup.maxStackCount)
                            continue;

                        StatusInstance preferred = other;
                        // The moment we find one that is suitable to replace, break out of the logic
                        switch (statusGroup.overridePriority)
                        {
                            case DataCenter.StatusGroupDefinition.OverridePriority.KeepCurrent:
                                return;
                            case DataCenter.StatusGroupDefinition.OverridePriority.RefreshCurrent:
                                other.ResetTimer();
                                return;
                            case DataCenter.StatusGroupDefinition.OverridePriority.LongestDuration:
                                if (newStatusInstance.duration > (other.duration - other.lifeTimer))
                                {
                                    preferred = newStatusInstance;
                                }
                                break;
                            case DataCenter.StatusGroupDefinition.OverridePriority.ShortestDuration:
                                if (newStatusInstance.duration < (other.duration - other.lifeTimer))
                                {
                                    preferred = newStatusInstance;
                                }
                                break;
                        }

                        if (preferred != other)
                        {
                            m_statusInstanceList.Remove(other);
                            m_statusInstanceList.Add(newStatusInstance);
                            other.EndStatus();
                            newStatusInstance.StartStatus();
                            return;
                        }
                    }
                }
            }

            // There were no conflicts
            m_statusInstanceList.Add(newStatusInstance);
            newStatusInstance.StartStatus();
        }

        public void Update(float time)
        {
            m_cachedStatusRemovalList.Clear();
            for (int i = m_statusInstanceList.Count - 1; i >= 0; --i)
            {
                m_statusInstanceList[i].Update(time);
                if (m_statusInstanceList[i].expired)
                {
                    m_cachedStatusRemovalList.Add(m_statusInstanceList[i]);
                }
            }

            for (int i = m_cachedStatusRemovalList.Count - 1; i >= 0; --i)
            {
                m_statusInstanceList.Remove(m_cachedStatusRemovalList[i]);
                m_cachedStatusRemovalList[i].EndStatus();
            }
        }

        public void OnEvent(string eventName, object source, object target)
        {
            int statusInstanceListCount = m_statusInstanceList.Count;
            for (int i = 0; i < statusInstanceListCount; i++)
            {
                m_statusInstanceList[i].ExecuteEvent(eventName, source, target);
            }
        }

        private Dictionary<string, float> m_modifiers = new Dictionary<string, float>(System.StringComparer.Ordinal)
        {
            {"default", 1.0f}
        };

        public Dictionary<string, float> modifiers
        {
            get { return m_modifiers; }
        }

        public void AddModifier(string modifier, float value)
        {
            if (m_modifiers.ContainsKey(modifier) == false)
                m_modifiers.Add(modifier, value);
            else
                m_modifiers[modifier] += value;
        }

        public float GetModifier(string modifier, float defaultValue)
        {
            if (m_modifiers.ContainsKey(modifier) == false)
            {
                m_modifiers.Add(modifier, defaultValue);
                return defaultValue;
            }
            else
                return m_modifiers[modifier];
        }
    };


}