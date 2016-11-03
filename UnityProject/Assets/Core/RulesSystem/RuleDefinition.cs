using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Linq;

namespace DataCenter
{
    public enum TargetType
    {
        TARGET, SOURCE, SELF
    };

    public class KeyValueData
    {
        [XmlAttribute]
        public string key { get { return _key; } set { _key = value; } }
        private string _key;

        [XmlAttribute]
        public string value { get { return _value; } set { _value = value; } }
        private string _value;
    };

    public class DataTable<T> : List<T>  where T : KeyValueData
    {
        private Dictionary<string, T> keyValueDataByKey = new Dictionary<string, T>();
        private List<string> keys = new List<string>();
        private List<T> values = new List<T>();

        public List<string> Keys
        {
            get { return keys; }
        }

        public List<T> Values
        {
            get { return values; }
        }

        public void CopyFrom(DataTable<T> dataTable)
        {
            int itemCount = dataTable.Values.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.Add(dataTable.Values[i]);
            }
        }

        new public void Add(T keyValueData)
        {
            if (keyValueData == null || string.IsNullOrEmpty(keyValueData.key))
                return;

            string key = keyValueData.key;

            if (keyValueDataByKey.ContainsKey(key) == false)
            {
                keyValueDataByKey.Add(key, null);
            }
            keyValueDataByKey[keyValueData.key] = keyValueData;
            if (keys.Contains(key))
            {
            }
            else
            {
                keys.Add(key);
                values.Add(keyValueData);
            }
        }

        static public DataTable<T> Merge(DataTable<T> t1, DataTable<T> t2)
        {
            DataTable<T> combined = new DataTable<T>();
            return combined;
        }

        public T GetValue(string key)
        {
            T ret;
            TryGetValue(key, out ret);
            return ret;
        }

        public bool TryGetValue(string key, out T keyValueData)
        {
            keyValueData = default(T);
            if (this.keyValueDataByKey.ContainsKey(key))
            {
                keyValueData = keyValueDataByKey[key];
                return true;
            }
            return false;
        }
    }

    public class RuleData : KeyValueData
    {
        [XmlElement("vfxtrigger")]
        public List<VFXDefinition> vfxtriggers = new List<VFXDefinition>();
    };

    public class RuleDefinition : DataObjectTemplate
    {
        [XmlAttribute]
        public string function { get { return _function; } set { _function = value; this.m_changeList.Add("function"); } }
        private string _function;

        [XmlElement]
        public TargetType target { get { return _target; } set { _target = value; this.m_changeList.Add("target"); } }
        private TargetType _target = TargetType.TARGET;

        [XmlElement("status")]
        public List<StatusDefinition> statusList = new List<StatusDefinition>();

        [XmlElement("rule")]
        public List<RuleDefinition> rulesList = new List<RuleDefinition>();

        [XmlElement("condition")]
        public List<ConditionDefinition> conditionList = new List<ConditionDefinition>();

        [XmlElement("data")]
        public DataTable<RuleData> ruleDataTable = new DataTable<RuleData>();
    };
}
