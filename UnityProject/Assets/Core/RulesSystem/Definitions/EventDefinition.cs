using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataCenter
{
    public class EventDefinition
    {
        [XmlAttribute]
        public string eventName;

        [XmlElement("rule")]
        public List<RuleDefinition> rulesList = new List<RuleDefinition>();
    };  

}