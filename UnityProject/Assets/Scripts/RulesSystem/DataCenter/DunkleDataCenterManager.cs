using UnityEngine;
using System.Collections;

namespace DataCenter
{
    public class DunkleDataCenterManager : DataCenterManager
    {
        public override void Log(string text)
        {
            DebugUtil.Log(DebugFlags.DataCenter, text);
        }

        public override void Error(string text)
        {
            DebugUtil.LogError(DebugFlags.DataCenter, text);
        }

        public override System.IO.TextReader LoadFile(string fileName)
        {
            //xmlFile = new StreamReader(DataCenterManager.Instance.FilePath + filename);
            //#if UNITY_EDITOR
            //        file = new System.IO.StreamReader(Application.dataPath + "/Resources/" + fileName);
            //#else
            Object asset = Resources.Load(fileName);
            if (asset == null)
            {
                DebugUtil.LogError(DebugFlags.DataCenter, "Could not find asset " + fileName);
                return null;
            }
            TextAsset xmlAsset = asset as TextAsset;
            if (xmlAsset == null)
            {
                DebugUtil.LogError(DebugFlags.DataCenter, "Could not cast asset " + fileName + " as TextAsset");
                return null;
            }
            return new System.IO.StringReader(xmlAsset.text);
            //#endif
        }
    }

}