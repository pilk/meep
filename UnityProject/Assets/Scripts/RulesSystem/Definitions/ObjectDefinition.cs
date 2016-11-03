using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

using DataCenter;


public partial class ObjectDefinition : DataObjectTemplate
{
    [XmlElement]
    public string objectName { get { return m_objectName; } set { m_objectName = value; this.m_changeList.Add("objectName"); } }
    private string m_objectName;

    [XmlElement]
    public int healthPoints { get { return m_healthPoints; } set { m_healthPoints = value; this.m_changeList.Add("healthPoints"); } }
    private int m_healthPoints;

    [XmlElement]
    public string model { get { return m_model; } set { m_model = value; this.m_changeList.Add("model"); } }
    private string m_model;

    [XmlElement]
    public string layer { get { return m_layer; } set { m_layer = value; this.m_changeList.Add("layer"); } }
    private string m_layer;

    [XmlElement("event")]
    public List<EventDefinition> events = new List<EventDefinition>();
};
