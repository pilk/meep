using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using DataCenter;

//public class SpawnObjectDefinition : DataTemplate
//{
//    [XmlElement]
//    public string spawner { get { return _spawner; } set { _spawner = value; this.changeList.Add("spawner"); } }
//    private string _spawner;

//    [XmlElement("allegiance")]
//    private List<int> allegianceList = new List<int>();

//    [XmlElement("status")]
//    public List<StatusDefinition> statusList = new List<StatusDefinition>();
//};

public class SpawnTriggerDefinition : DataObjectTemplate
{

};

public class SceneDefinition : ProbabilityTreeDefinition
{
    [XmlElement("event")]
    public List<EventDefinition> m_events = new List<EventDefinition>();
};
