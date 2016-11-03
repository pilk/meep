using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace DataCenter
{
    sealed public class XMLData<T> : DataCenterModule<T> where T : class
    {
        public delegate void XMLDataEvent();
        public XMLDataEvent OnFileLoaded = null;
        public XMLDataEvent OnDeserializationComplete = null;

        private static XmlSerializer m_serializer = null;
        private Dictionary<string, string> m_loadedDataByFileName = new Dictionary<string, string>();


        public XMLData(DataLog logFunction, DataLog errorFunction)
        {
            LogError = errorFunction;
            Log = logFunction;

            m_serializer = new XmlSerializer(typeof(T));
            m_serializer.UnknownElement += new XmlElementEventHandler(serializer_UnknownElement);
            m_serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            m_serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
        }

        public void LoadFile(string filename, string dataPath)
        {
            if (string.IsNullOrEmpty(filename))
            {
                LogError("Was given empty string as a filename");
                return;
            }

            TextReader xmlFile = DataCenterManager.Instance.LoadFile(dataPath + filename);
            if (xmlFile == null)
            {
                LogError("Could not find file " + filename);
                return;
            }
            CacheFileContents(filename, xmlFile);
        }

        public void CacheFileContents(string filename, TextReader xmlFile)
        {
            string data = xmlFile.ReadToEnd();
            if (string.IsNullOrEmpty(data) == false)
            {
                m_loadedDataByFileName.Add(filename, data);
            }
            else
            {
                LogError(filename + " was empty");
            }
            xmlFile.Close();
            Log("Loaded file [" + filename + "]");

            if (OnFileLoaded != null)
            {
                OnFileLoaded();
            }
        }

        public void SaveFile(string fileName, T data)
        {
            StreamWriter streamWriter = new StreamWriter(DataCenterManager.Instance.filePath + "/" + fileName);
            m_serializer.Serialize(streamWriter, data);
            streamWriter.Close();
        }

        public void Serialize(string fileName, T data)
        {
            StreamWriter streamWriter = new StreamWriter(DataCenterManager.Instance.filePath + "/" + fileName);
            m_serializer.Serialize(streamWriter, data);
            streamWriter.Close();
        }

        public T Deserialize(string fileName)
        {
            T serializedData = null;
            if (m_loadedDataByFileName.ContainsKey(fileName) == false)
            {
                LogError("No such file " + fileName + " was loaded");
                return null;
            }

            try
            {
                Log("Reading [" + fileName + "]");
                serializedData = (T)m_serializer.Deserialize(new System.IO.StringReader(m_loadedDataByFileName[fileName]));
            }
            catch (System.Exception exception)
            {
                LogError("Was not able to parse XML file " + fileName + "\n" + exception.ToString());
                return default(T);
            }

            Log("Completed reading [" + fileName + "]");
            if (OnDeserializationComplete != null)
            {
                OnDeserializationComplete();
            }
            return serializedData;
        }


        private void serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            LogError("Object Being Deserialized: " + e.ObjectBeingDeserialized.GetType().Name);
            LogError("Unknown Element:" + "\t" + e.Element.Name + "\t" + e.ToString() + "\n\tline: " + e.LineNumber + " position: " + e.LinePosition);
        }

        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            LogError("Object Being Deserialized: " + e.ObjectBeingDeserialized.GetType().Name);
            LogError("Unknown Node:" + "\t" + e.Name + "\t" + e.ToString() + "\n\tline: " + e.LineNumber + " position: " + e.LinePosition);
        }

        private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            LogError("Object Being Deserialized: " + e.ObjectBeingDeserialized.GetType().Name);
            LogError("Unknown Attribute:" + "\t" + e.Attr.Name + "\t" + e.ToString() + "\n\tline: " + e.LineNumber + " position: " + e.LinePosition);
        }
    };
}