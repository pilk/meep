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
    public class NoCopyFromAttribute : Attribute { }

    abstract public class DataCenterManager
    {
        protected System.Action OnDataReady = null;
        protected bool m_dataReady = false;

        protected string m_filePath = null;
        protected string m_entryFileName = null;

        protected XMLData<DataTypes> m_xmlModule = null;

        // All registered types of data
        protected Dictionary<string, System.Type> m_registeredDataTemplateTypes = new Dictionary<string, Type>(System.StringComparer.Ordinal);

        // All registered data
        protected List<DataObjectTemplate> m_dataList = new List<DataObjectTemplate>(256); // Expecting a large set of data

        // All registered data objects by name and ID
        protected Dictionary<System.Type, Dictionary<string, DataObjectTemplate>> m_dataByName = new Dictionary<Type, Dictionary<string, DataObjectTemplate>>();
        protected Dictionary<System.Type, Dictionary<int, DataObjectTemplate>> m_dataByID = new Dictionary<Type, Dictionary<int, DataObjectTemplate>>();

        static protected DataCenterManager s_instance = null;
        static public DataCenterManager Instance
        {
            get { return s_instance; }
        }


        public string filePath
        {
            get { return m_filePath; }
        }

        public void CallWhenDataReady(System.Action callback)
        {
            if (m_dataReady == false)
            {
                OnDataReady += callback;
            }
            else
            {
                callback();
            }
        }

        public void Initialize(string entryFileName, string filePath)
        {
            if (s_instance != null)
            {
                s_instance.Error("Attempting to initialize DataCenterManager more than once");
                return;
            }

            s_instance = this;
            m_xmlModule = new XMLData<DataTypes>(this.Log, this.Error);
            m_filePath = filePath;
            m_entryFileName = entryFileName;

            // Load main file to know which other files to include
            DataTypes filesData = LoadXMLFile(m_entryFileName);

            if (filesData == null || filesData.includeList == null || filesData.includeList.Count == 0)
            {
                Error("There were no files to parse?");
                return;
            }

            // Load all the other files
            using (Utilities.Benchmark.Start("LoadByXML"))
            {
                LoadXMLFiles(filesData.includeList);
            }

            if (OnDataReady != null)
            {
                OnDataReady();
                OnDataReady = null;
            }

            m_dataReady = true;

            Log("DataCenterManager initialization complete");
        }

        private DataTypes LoadXMLFile(string fileName)
        {
            DataTypes filesData = null;
            using (Utilities.Benchmark.Start("Reading " + fileName + " file"))
            {
                m_xmlModule.LoadFile(fileName, m_filePath);
                filesData = m_xmlModule.Deserialize(fileName);
            }
            return filesData;
        }

        private DataTypes LoadXMLFiles(List<FileDefinition> files)
        {
            // Begin parsing all files
            DataTypes master = new DataTypes();
            using (Utilities.Benchmark.Start("Reading XML files"))
            {
                int numFiles = files.Count;
                for (int i = 0; i < numFiles; i++)
                {
                    using (Utilities.Benchmark.Start("Reading " + files[i].file + " XML file"))
                    {
                        m_xmlModule.LoadFile(files[i].file, m_filePath);
                        DataTypes moreData = m_xmlModule.Deserialize(files[i].file);
                        if (moreData != null)
                        {
                            master.Add(moreData);
                        }
                    }
                }
            }


            // Assess copy inheritances
            // Walk through all the pieces of data we have and construct a copyTo tree
            using (Utilities.Benchmark.Start("Assessing inhertiances"))
            {
                Log("Assessing inheritances");
                int numDataList = m_dataList.Count;
                List<DataObjectTemplate> copyRoots = new List<DataObjectTemplate>();
                for (int i = 0; i < numDataList; i++)
                {
                    // Does not copy from anything but could potentially copy to something
                    if (string.IsNullOrEmpty(m_dataList[i].copyFrom))
                    {
                        if (!string.IsNullOrEmpty(m_dataList[i].name))
                        {
                            copyRoots.Add(m_dataList[i]);
                        }
                        continue;
                    }



                    DataObjectTemplate copyFrom;
                    if (TryGetByName<DataObjectTemplate>(m_dataList[i].copyFrom, out copyFrom) == false)
                    {
                        Error("Could not copy from " + m_dataList[i].copyFrom + " because it does not exist");
                    }
                    else
                    {
                        if (copyFrom.GetType() == m_dataList[i].GetType())
                        {
                            //Log("Copy from : " + copyFrom.name + " to " + m_dataList[i].name);
                            copyFrom.m_copyToList.Add(m_dataList[i]);
                        }
                    }
                }

                int numCopyRoots = copyRoots.Count;
                for (int i = 0; i < numCopyRoots; i++)
                {
                    copyRoots[i].ExecuteCopyTo();
                }
                Log("Completed copy");
            }
            return master;
        }
        

        public bool TryGetByName<T>(string name, out T returnValue) where T : DataObjectTemplate
        {
            return TryGetByName<T>(name, out returnValue, default(T));
        }

        public bool TryGetByName<T>(string name, out T returnValue, T defaultValue) where T : DataObjectTemplate
        {
            System.Type type = typeof(T);
            if (m_dataByName.ContainsKey(type) == false)
            {
                returnValue = defaultValue;
                return false;
            }

            if (m_dataByName[type].ContainsKey(name) == false)
            {
                returnValue = defaultValue;
                return false;
            }

            returnValue = (T)m_dataByName[type][name];
            return true;
        }

        public void RegisterData(DataObjectTemplate data)
        {
            System.Type baseType = typeof(DataObjectTemplate);

            if (m_registeredDataTemplateTypes.ContainsKey(baseType.ToString()) == false)
                m_registeredDataTemplateTypes.Add(baseType.ToString(), baseType);

            if (m_dataByName.ContainsKey(baseType) == false)
                m_dataByName.Add(baseType, new Dictionary<string, DataObjectTemplate>());
            if (m_dataByID.ContainsKey(baseType) == false)
                m_dataByID.Add(baseType, new Dictionary<int, DataObjectTemplate>());

            m_dataByName[baseType][data.name] = data;
            m_dataByID[baseType][data.ID] = data;


            System.Type type = data.GetType();
            if (m_dataByName.ContainsKey(type) == false)
                m_dataByName.Add(type, new Dictionary<string, DataObjectTemplate>(System.StringComparer.Ordinal));
            if (m_dataByID.ContainsKey(type) == false)
                m_dataByID.Add(type, new Dictionary<int, DataObjectTemplate>());
            if (m_dataByName[type].ContainsKey(data.name) == false)
                m_dataByName[type].Add(data.name, null);
            if (m_dataByID[type].ContainsKey(data.ID) == false)
                m_dataByID[type].Add(data.ID, null);


            m_dataByName[type][data.name] = data;
            m_dataByID[type][data.ID] = data;
            m_dataList.Add(data);
        }

        public void UnregisterData(DataObjectTemplate data)
        {
            if (string.IsNullOrEmpty(data.name))
                return;

            System.Type type = data.GetType();

            if (m_dataByName.ContainsKey(type) == false)
                return;

            if (m_dataByName[type].ContainsKey(data.name) == false)
                return;

            m_dataByName[type].Remove(data.name);
            m_dataByID[type].Remove(data.ID);
            m_dataList.Remove(data);
        }

        abstract public void Log(string text);
        abstract public void Error(string text);
        abstract public TextReader LoadFile(string fileName);
    };
}
