using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace DataCenter
{
    abstract public partial class DataObjectTemplate
    {
        private static int ID_INCREMENT = 0;
        public DataObjectTemplate()
        {
            // Default anonymous names
            this.name = (ID_INCREMENT++).ToString();
        }

        [XmlAttribute]
        [NoCopyFromAttribute]
        public string name
        {
            get { return m_name; }
            set
            {
                DataCenterManager.Instance.UnregisterData(this);
                m_name = value;
                m_id = Utilities.Jenkins96.GetHash(m_name);
                DataCenterManager.Instance.RegisterData(this);
            }
        }

        [NoCopyFromAttribute]
        protected string m_name;

        [NoCopyFromAttribute]
        public int ID
        {
            get { return m_id; }
        }

        [NoCopyFromAttribute]
        protected int m_id;

        [XmlAttribute]
        [NoCopyFromAttribute]
        public string copyFrom
        {
            set { m_copyFrom = value; }
            get { return m_copyFrom; }
        }

        [NoCopyFromAttribute]
        protected string m_copyFrom;

        [NoCopyFromAttribute]
        public List<DataObjectTemplate> m_copyToList = new List<DataObjectTemplate>();

        [NoCopyFromAttribute]
        protected List<string> m_changeList = new List<string>();

        [NoCopyFromAttribute]
        public List<string> changeList
        {
            get { return m_changeList; }
        }


        [XmlElement("tag")]
        public List<string> tagList = new List<string>();


        public void LogFields()
        {
            string log = "";
            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0, fieldCount = fields.Length; i < fieldCount; ++i)
            {
                log += fields[i].Name + " : " + fields[i].GetValue(this) + "\n";
            }
            DataCenter.DataCenterManager.Instance.Log(log);
        }

        public void ExecuteCopyTo()
        {
            //System.Type thisType = this.GetType();

            int numCopyTo = m_copyToList.Count;
            for (int i = 0; i < numCopyTo; i++)
            {
                DataObjectTemplate copyTo = m_copyToList[i];

                // Copy all fields of list types
                FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                int fieldCount = fields.Length;
                for (int f = 0; f < fieldCount; f++)
                {
                    FieldInfo field = fields[f];
                    if (field.IsDefined(typeof(NoCopyFromAttribute), false))
                        continue;

                    object newField = field.GetValue(this);         // Take value from here
                    if (newField == null) continue;
                    object oldField = field.GetValue(copyTo);       // Place into here

                    System.Type fieldType = newField.GetType();
                    MethodInfo mi = fieldType.GetMethod("CopyFrom", new Type[] { fieldType });

                    // Has a specific copyFrom override
                    // This only happens if we're asking an Object class to copy from another
                    if (mi != null)
                    {
                        mi.Invoke(oldField, new object[] { newField });
                    }
                    // Is a type of list, combine both
                    else if (oldField is IList)
                    {
                        IList oldList = oldField as IList;
                        IList newList = newField as IList;
                        foreach (object o in newList)
                        {
                            oldList.Add(o);
                        }
                    }
                    // Is a normal object value, override
                    else
                    {
                        field.SetValue(copyTo, newField);
                    }
                }

                // Copy all propeties 
                PropertyInfo[] properties = this.GetType().GetProperties();
                int propertyCount = properties.Length;
                for (int p = 0; p < propertyCount; p++)
                {
                    PropertyInfo property = properties[p];
                    if (property.IsDefined(typeof(NoCopyFromAttribute), false))
                        continue;

                    object newProperty = property.GetValue(this, null);
                    object currentProperty = property.GetValue(copyTo, null);

                    System.Type propertyType = property.GetType();
                    MethodInfo mi = propertyType.GetMethod("CopyFrom", new Type[] { propertyType });
                    if (mi != null)
                    {
                        // Use custom CopyFrom function when provided
                        mi.Invoke(currentProperty, new object[] { newProperty });
                    }
                    else
                    {
                        //object oldProperty = property.GetValue(copyTo, null);
                        // Only overwrite variables that were not set as changed already
                        if (copyTo.changeList.Contains(property.Name) == false) // && oldProperty.Equals(newProperty) == false)
                        {
                            property.SetValue(copyTo, newProperty, null);
                        }
                    }


                    // Note : This is a shallow copy, to avoid this, create a new instance of that field and set it
                    // Although the copy should never be changed, this should not be a problem
                }

                // Apply copy to with the children of this item
                copyTo.ExecuteCopyTo();
            }
        }
    };

}