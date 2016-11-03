using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DataCenter
{
    public partial class DataTypes
    {
        [XmlElement("include")]
        public List<FileDefinition> includeList = new List<FileDefinition>();

        [XmlElement("rule")]
        public List<RuleDefinition> rulesList = new List<RuleDefinition>();

        [XmlElement("condition")]
        public List<ConditionDefinition> conditionList = new List<ConditionDefinition>();

        [XmlElement("status")]
        public List<StatusDefinition> statusList = new List<StatusDefinition>();

        [XmlElement("statusGroup")]
        public List<StatusGroupDefinition> statusGroupList = new List<StatusGroupDefinition>();

        public void Add(DataTypes other)
        {
            this.includeList.AddRange(other.includeList);
        }
    };
}