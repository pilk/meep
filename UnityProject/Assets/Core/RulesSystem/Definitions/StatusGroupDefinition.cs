using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataCenter
{
    public class StatusGroupDefinition : DataObjectTemplate
    {
        private int m_maxStackCount = 1;  // How many of the same type of status can affect a target
        [XmlElement]
        public int maxStackCount
        {
            get { return m_maxStackCount; }
            set { m_maxStackCount = value; this.changeList.Add("maxStackCount"); }
        }

        public enum OverridePriority
        {
            KeepCurrent,
            RefreshCurrent,
            LongestDuration,
            ShortestDuration,
        }

        private OverridePriority m_overridePriority = OverridePriority.KeepCurrent;
        public OverridePriority overridePriority
        {
            get { return m_overridePriority; }
            set { m_overridePriority = value; this.changeList.Add("overridePiority"); }
        }
    };
}