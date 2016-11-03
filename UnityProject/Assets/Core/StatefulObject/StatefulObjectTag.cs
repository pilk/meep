using UnityEngine;
using System.Collections;

public class StatefulObjectTag : MonoBehaviour
{
    public GameObject m_target = null;
    public string[] m_tagIDs = new string[0];

    private void Reset()
    {
        if (m_target == null)
        {
            m_target = this.gameObject;
        }
    }
}
