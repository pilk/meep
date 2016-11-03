using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour 
{
    public CameraTag m_cameraTarget = null;
    public float m_cameraTransitionTime = 0.3f;
    private Camera m_camera = null;

    static private CameraController s_instance = null;
    static public CameraController Instance
    {
        get { return s_instance; }
    }

    private void Awake()
    {
        s_instance = this;
        m_camera = this.GetComponent<Camera>();
    }

    public void SetCamera(string cameraTagID)
    {
        CameraTag cameraTag = CameraTag.Get(cameraTagID);
        if (cameraTag != null)
        {
            SetCamera(cameraTag);
        }
    }

    public void SetCamera(CameraTag cameraTag)
    {
        m_cameraTarget = cameraTag;
        TransitionToTargetCamera();
    }

    public void TransitionToTargetCamera()
    {
        // Blend the source camera to the target camera settings
        Camera source = this.m_camera;
        Camera target = m_cameraTarget.camera;

        //source.DOOrthoSize(target.orthographicSize, m_cameraTransitionTime);
        //source.transform.DOMoveX(target.transform.position.x, m_cameraTransitionTime);
        //source.transform.DOMoveY(target.transform.position.y, m_cameraTransitionTime);
        //source.transform.DOMoveZ(target.transform.position.z, m_cameraTransitionTime);
    }
}
