using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DataCenter
{
    public class StatusDefinition : DataObjectTemplate
    {
        [XmlElement]
        public float duration { get { return _duration; } set { _duration = value; this.m_changeList.Add("duration"); } }
        private float _duration = -1;

        [XmlElement]
        public float tickIntervalTime { get { return _tickIntervalTime; } set { _tickIntervalTime = value; this.m_changeList.Add("tickIntervalTime"); } }
        private float _tickIntervalTime = 1;

        [XmlElement("statusGroupName")]
        public List<string> statusGroupNameList = new List<string>();

        [XmlElement("event")]
        public List<EventDefinition> events = new List<EventDefinition>();
    };
}
