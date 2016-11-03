using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GroundObject : MonoBehaviour
{
#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isPlaying)
            return;
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, -Vector3.up * 1000.0f, out hit))
        {
            this.transform.position = hit.point;
        }
    }
#endif
}
