using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

using DataCenter;


public partial class CharacterDefinition : ObjectDefinition
{
    [XmlElement]
    public AllegianceFlags allegiance { get { return m_allegiance; } set { m_allegiance = value; this.m_changeList.Add("allegiance"); } }
    private AllegianceFlags m_allegiance;

    [XmlElement]
    public float speed { get { return m_speed; } set { m_speed = value; this.m_changeList.Add("speed"); } }
    private float m_speed;
};
