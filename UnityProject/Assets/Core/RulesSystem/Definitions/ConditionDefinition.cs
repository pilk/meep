using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DataCenter
{
    public class ConditionDefinition : DataObjectTemplate
    {
        public enum ConditionType { AND, OR };

        [XmlAttribute]
        public ConditionType type
        {
            get { return m_type; }
            set { m_type = value; m_changeList.Add("type"); }
        }
        private ConditionType m_type = ConditionType.AND;

        [XmlAttribute]
        public bool result
        {
            get { return m_result; }
            set { m_result = value; m_changeList.Add("result"); }
        }
        private bool m_result = true;

        [XmlAttribute]
        public string function
        {
            get { return m_function; }
            set { m_function = value; m_changeList.Add("function"); }
        }
        private string m_function = null;

        [XmlElement]
        public DataCenter.TargetType target
        {
            get { return m_target; }
            set { m_target = value; m_changeList.Add("target"); }
        }
        private DataCenter.TargetType m_target = TargetType.TARGET;


        [XmlElement]
        public string value
        {
            get { return m_value; }
            set { m_value = value; m_changeList.Add("value"); }
        }
        private string m_value;


        [XmlElement("condition")]
        public List<ConditionDefinition> conditionList = new List<ConditionDefinition>();
    };
}
