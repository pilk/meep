using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class EntityMovement : EntityComponent 
{
    public float m_speed = 1.0f;
    public float m_closingThreshold = 0.1f;

    private Vector3? m_desiredPosition = null;
    private Transform m_transform = null;
    private Rigidbody m_rigidbody = null;

    public bool isMoving
    {
        get { return m_desiredPosition.HasValue; }
    }

    protected override void Start()
    {
        base.Start();
        m_transform = this.GetComponent<Transform>();
        m_rigidbody = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (m_desiredPosition.HasValue)
        {
            Vector3 desiredPosition = m_desiredPosition.Value;

            Vector3 fromTo = (desiredPosition - m_transform.position);

            // Move towards position
            Vector3 velocity = fromTo.normalized * m_speed;
            velocity.y = 0.0f;
            m_rigidbody.velocity = velocity;
            m_transform.LookAt(this.transform.position + velocity, Vector3.up);
            m_entityController.Move(velocity);

            // Has reached destination?
            if (fromTo.sqrMagnitude < m_closingThreshold * m_closingThreshold)
            {
                m_desiredPosition = null;
                m_rigidbody.velocity = Vector3.zero;
                
                m_entityController.FinishedMoving();
            }
        }
    }

    public void GoToPosition(Vector3 position)
    {
        Vector3 fromTo = (position - m_transform.position);
        // Has reached destination?
        if (fromTo.sqrMagnitude > m_closingThreshold * m_closingThreshold)
        {
            m_desiredPosition = position;
        }
    }

    public bool CloseToPosition(Vector3 position, float slack = 1.0f)
    {
        return ((position - m_transform.position).sqrMagnitude < m_closingThreshold * m_closingThreshold + (slack * slack));
    }


    private void OnDrawGizmosSelected()
    {
        if (m_desiredPosition.HasValue)
        {
            Debug.DrawLine( this.transform.position, m_desiredPosition.Value, Color.red );
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_desiredPosition.Value, 0.2f);
        }
    }
}
