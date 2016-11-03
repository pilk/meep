using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class VFXDefinition : DataCenter.DataObjectTemplate
{
    public enum StopType
    {
        Stop,
        StopImmediately,
    };

    [XmlElement]
    public string vfx { get { return m_vfx; } set { m_vfx = value; this.m_changeList.Add("vfx"); } }
    private string m_vfx;

    [XmlElement]
    public float duration { get { return m_duration; } set { m_duration = value; this.m_changeList.Add("duration"); } }
    private float m_duration;

    [XmlElement]
    public StopType stopType { get { return m_stopType; } set { m_stopType = value; this.m_changeList.Add("stopType"); } }
    private StopType m_stopType;
};