using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class CameraMovementSettings : MonoBehaviour
{
    public Transform m_followTarget = null;
    public Vector3 m_targetOffset = new Vector3(0.0f, 0.0f, 1.0f);
    public float m_borderSizePercentage = 2.0f;
    public float m_smoothTime = 0.3f;

    private float m_borderSize = 10.0f;
    private Camera m_camera = null;
    private Vector3? m_desiredPosition = null;
    private Vector3 m_cameraVelocity = Vector3.zero;

    private void Awake()
    {
        m_camera = this.GetComponent<Camera>();
    }

    private void Start()
    {
        GameLoader.CallAfterCompletion(this.Setup);
    }

    private void Setup()
    {
        GlobalTag.RunActionOnObject("player", SetTarget);
        GlobalTag.RegisterActionForObject("player", SetTarget);
    }

    public void SetTarget(GameObject player)
    {
        m_followTarget = player.transform;
    }

    private void OnDestroy()
    {
        GlobalTag.UnregisterActionForObject("player", SetTarget);
    }

    private void LateUpdate()
    {
        if (m_followTarget == null)
            return;

        m_borderSize = (new Vector2(Screen.width, Screen.height) * 0.01f * m_borderSizePercentage).magnitude;

        if (m_desiredPosition.HasValue == false)
        {
            Rect cameraBoundaries = new Rect(m_borderSize, m_borderSize, Screen.width - m_borderSize * 2, Screen.height - m_borderSize * 2);
            Vector3 targetPosition = m_followTarget.position;
            targetPosition += m_followTarget.forward * m_targetOffset.z;
            targetPosition += m_followTarget.right * m_targetOffset.x;
            targetPosition += m_followTarget.up * m_targetOffset.y;
            Vector2 screenTargetPosition = m_camera.WorldToScreenPoint(targetPosition);
            if (cameraBoundaries.Contains(screenTargetPosition) == false)
            {
                Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                Vector2 dir = (screenTargetPosition - center).normalized;
                Vector2 adjustedPosition = center + dir * m_borderSize;
                m_desiredPosition = m_camera.ScreenToWorldPoint(adjustedPosition);
            }
            else
            {
                m_desiredPosition = null;
            }
        }

        if (m_desiredPosition.HasValue)
        {
            transform.position = Vector3.SmoothDamp(transform.position, m_desiredPosition.Value, ref m_cameraVelocity, m_smoothTime);

            if ((m_desiredPosition.Value - this.transform.position).sqrMagnitude < 0.5f)
                m_desiredPosition = null;
        }
    }

    public bool TestBoundaries(Vector3 position, float radius)
    {
        Rect rect = new Rect(m_borderSize, m_borderSize, Screen.width - m_borderSize * 2, Screen.height - m_borderSize * 2);
        return rect.Contains(m_camera.WorldToScreenPoint(position));
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 topLeft = new Vector3(m_borderSize, Screen.height - m_borderSize, 1.0f);
        Vector3 bottomRight = new Vector3(Screen.width - m_borderSize, m_borderSize, 1.0f);

        List<Vector3> positions = new List<Vector3>();
        positions.Add(GetComponent<Camera>().ScreenToWorldPoint(topLeft));
        positions.Add(GetComponent<Camera>().ScreenToWorldPoint(bottomRight));

        positions.Add(GetComponent<Camera>().ScreenToWorldPoint(new Vector3(topLeft.x, bottomRight.y, 1.0f)));
        positions.Add(GetComponent<Camera>().ScreenToWorldPoint(new Vector3(topLeft.x, topLeft.y, 1.0f)));
        positions.Add(GetComponent<Camera>().ScreenToWorldPoint(new Vector3(bottomRight.x, topLeft.y, 1.0f)));
        positions.Add(GetComponent<Camera>().ScreenToWorldPoint(bottomRight));
        DebugUtil.DrawLines(positions.ToArray(), Color.red);

        if (m_followTarget != null)
        {
            Vector3 targetPosition = m_followTarget.position;
            targetPosition += m_followTarget.forward * m_targetOffset.z;
            targetPosition += m_followTarget.right * m_targetOffset.x;
            targetPosition += m_followTarget.up * m_targetOffset.y;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
        }
    }
}
