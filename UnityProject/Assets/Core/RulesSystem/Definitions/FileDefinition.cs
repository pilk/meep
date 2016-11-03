using System.Collections;
using System.Xml.Serialization;

public class FileDefinition : DataCenter.DataObjectTemplate
{
    protected string _file;

    [XmlAttribute]
    public string file
    {
        set { _file = value; }
        get { return _file; }
    }
};