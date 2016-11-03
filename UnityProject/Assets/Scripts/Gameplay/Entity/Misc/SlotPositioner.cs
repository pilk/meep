using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlotPositioner : MonoBehaviour
{
    public class Slot
    {
        public Slot(int index)
        {
            this.index = index;
        }

        public readonly int index;
        private bool m_occupied = false;
        public bool occupied
        {
            get { return m_occupied; }
        }

        public void Occupy()
        {
            m_occupied = true;
        }

        public void Vacant()
        {
            m_occupied = false;
        }
    };

    public int m_slotCount = 8;
    public float m_radius = 1.0f;
    private Slot[] m_slots;

    private void Awake()
    {
        m_slots = new Slot[m_slotCount];
        for (int i = m_slotCount - 1; i >= 0; --i)
        {
            m_slots[i] = new Slot(i);
        }
    }

    public Slot FindAvailableSlot()
    {
        List<Slot> availabeSlots = new List<Slot>(m_slotCount);
        for (int i = m_slotCount - 1; i >= 0; --i)
        {
            if (m_slots[i].occupied == false)
                availabeSlots.Add(m_slots[i]);
        }

        if (availabeSlots.Count > 0)
        {
            return availabeSlots[Random.Range(0, availabeSlots.Count)];
        }
        return null;
    }

    public void RunActionOnSlots(System.Action<Slot> action)
    {
        for (int i = m_slotCount - 1; i >= 0; --i)
        {
            action(m_slots[i]);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 v = this.transform.forward * m_radius;
        float angle = 360.0f / m_slotCount;
        Gizmos.color = Color.red;
        for (int i = m_slotCount - 1; i >= 0; --i)
        {
            v = Quaternion.AngleAxis(angle, Vector3.up) * v;

            Color prevColor = Gizmos.color;
            if (m_slots != null)
            {
                if (m_slots[i].occupied) Gizmos.color = Color.green;
            }

            Gizmos.DrawWireCube(this.transform.position + v, new Vector3(0.2f, 0.0f, 0.2f));
            Gizmos.color = prevColor;
        }

        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(this.transform.position, this.transform.up, 1f);
    }

    public Vector3 GetPosition(Slot slot)
    {
        Vector3 v = this.transform.forward * m_radius;
        float angle = 360.0f / m_slotCount * slot.index;
        v = Quaternion.AngleAxis(angle, Vector3.up) * v;
        return this.transform.position + v;
    }
}
