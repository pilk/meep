using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class CameraTag : MonoBehaviour 
{
    static private Dictionary<string, CameraTag> s_instances = new Dictionary<string, CameraTag>(System.StringComparer.Ordinal);

    static public CameraTag Get(string tagID)
    {
        if (s_instances.ContainsKey(tagID))
        {
            return s_instances[tagID];
        }
        return null;
    }


    public string m_tagID;
    private Camera m_camera = null;

    public Camera camera
    {
        get { return m_camera; }
    }

    private void Reset()
    {
        m_tagID = this.gameObject.name;
    }

    private void Awake()
    {
        s_instances.Add(m_tagID, this);
        m_camera = this.GetComponent<Camera>();
        m_camera.enabled = false;
    }

    private void OnDestroy()
    {
        s_instances.Remove(m_tagID);
    }
}
