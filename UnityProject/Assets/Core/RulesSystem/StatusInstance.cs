using System;
using System.Collections;
using System.Collections.Generic;

namespace Rules
{
    sealed public class StatusInstance
    {
        private bool m_expired = false;

        private DataCenter.StatusDefinition m_statusDefinition = null;
        private object m_statusSource = null;
        private object m_statusTarget = null;
        private float m_lifeTimer = 0.0f;
        private int m_tickCount = 0;
        private EventListHelper m_eventListHelper = null;



        public bool expired { get { return m_expired; } }
        public DataCenter.StatusDefinition statusDefinition { get { return m_statusDefinition; } }
        public float duration { get { return m_statusDefinition.duration; } }


        public float lifeTimer
        {
            get { return m_lifeTimer; }
        }

        private StatusInstance() { }    // This constructor is not allowed, use custom contructor instead
        public StatusInstance(DataCenter.StatusDefinition statusDefinition, object source, object target)
        {
            m_eventListHelper = new EventListHelper(statusDefinition.events);
            m_statusSource = source;
            m_statusTarget = target;
            this.m_statusDefinition = statusDefinition;
        }

        public void Update(float time)
        {
            if (m_expired == true)
                return;

            m_lifeTimer += time;           
            if (m_lifeTimer >= (m_tickCount + 1) * this.m_statusDefinition.tickIntervalTime)
            {
                this.ExecuteEvent("tick", m_statusSource, m_statusTarget);
                m_tickCount++;
            }

            if (m_statusDefinition.duration != -1)
            {
                if (m_lifeTimer >= m_statusDefinition.duration)
                {
                    this.ExecuteEvent("status_expired", m_statusSource, m_statusTarget);
                    m_expired = true;
                }
            }
        }

        public void ExecuteEvent(string eventName, object source, object target)
        {
            if (m_expired == true)
                return;

            RulesSystem.Instance.ExecuteEvent(eventName, this.m_statusDefinition.events, source, target);
        }

        public void StartStatus()
        {
            ExecuteEvent("start", m_statusSource, m_statusTarget);
        }

        public void EndStatus()
        {
            ExecuteEvent("remove", m_statusSource, m_statusTarget);
        }

        public void ResetTimer()
        {
            m_lifeTimer = 0.0f;
        }
    };


    sealed public class EventListHelper
    {
        private Dictionary<string, List<DataCenter.EventDefinition>> m_eventsByName = new Dictionary<string, List<DataCenter.EventDefinition>>(StringComparer.Ordinal);

        private EventListHelper() { }
        public EventListHelper(List<DataCenter.EventDefinition> events)
        {
            foreach (DataCenter.EventDefinition eventDef in events)
            {
                if (this.m_eventsByName.ContainsKey(eventDef.eventName) == false)
                    this.m_eventsByName.Add(eventDef.eventName, new List<DataCenter.EventDefinition>());

                this.m_eventsByName[eventDef.eventName].Add(eventDef);
            }
        }

        public void ExecuteEvent(string eventName, object source, object target)
        {
            if (m_eventsByName.ContainsKey(eventName))
            {
                for (int i = 0, eventListCount = m_eventsByName[eventName].Count; i < eventListCount; i++)
                {
                    RulesSystem.Instance.ExecuteRulesList(m_eventsByName[eventName][i].rulesList, source, target);
                }
            }
        }
    };
}