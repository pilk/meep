using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;
using System.Text;

namespace DataCenter
{
    sealed public class JSONData<T> : DataCenterModule<T> where T : class
    {
        private JsonWriter m_jsonWriter;
        private JsonReader m_jsonReader;
        private System.Text.StringBuilder m_stringBuilder;
        private Dictionary<string, string> m_fileDataByName = new Dictionary<string, string>(System.StringComparer.Ordinal);

        public Dictionary<string, string> fileDataByName
        {
            get { return m_fileDataByName; }
        }

        public JSONData(DataLog logFunction, DataLog errorFunction)
        {
            LogError = errorFunction;
            Log = logFunction;

            m_stringBuilder = new StringBuilder();
            JsonWriterSettings writerSettings = new JsonWriterSettings();
            writerSettings.PrettyPrint = true;
            m_jsonWriter = new JsonWriter(m_stringBuilder, writerSettings);

            JsonReaderSettings readerSettings = new JsonReaderSettings();
            m_jsonReader = new JsonReader(m_stringBuilder, readerSettings);
        }

        public void LoadFile(string fileName)
        {
            FileStream stream = new FileStream(DataCenterManager.Instance.filePath + fileName, FileMode.OpenOrCreate, FileAccess.Read);
            if (stream == null)
            {
                LogError("Was not able to open file " + fileName);
                return;
            }

            if (stream.Length > 0)
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                string data = Encoding.UTF8.GetString(bytes);
                m_fileDataByName.Add(fileName, data);
            }

            stream.Close();
        }

        public void SaveFile(string fileName, T data)
        {
            StreamWriter streamWriter = new StreamWriter(DataCenterManager.Instance.filePath + "/" + fileName);
            if (streamWriter == null)
            {
                LogError("Was not able to open file " + fileName);
                return;
            }

            string dataString = Serialize(fileName, data);
            //byte[] bytes = Encoding.UTF8.GetBytes(dataString);
            m_fileDataByName[fileName] = dataString;
            streamWriter.Write(dataString);
            streamWriter.Close();
        }

        public T Deserialize(string data)
        {
            try
            {
                //dynamic deserializedObject = m_jsonReader.Read(data);

                //IDictionary<string, object> dict = (IDictionary<string, object>)deserializedObject;
                //foreach (var key in dict.Keys)
                //{
                //    Console.WriteLine(key);
                //}
                //return (T)m_jsonReader.Read(data);
            }
            catch (System.Exception ex)
            {
                LogError(ex.ToString());
            }
            return default(T);
        }

        public string Serialize(string fileName, T data)
        {
            try
            {
                m_jsonWriter.Write(data);
                return m_stringBuilder.ToString();
            }
            catch (System.Exception ex)
            {
                LogError(ex.ToString());
                return "";
            }
        }
    };
}