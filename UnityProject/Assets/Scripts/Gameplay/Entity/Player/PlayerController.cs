using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    [SerializeField]
    [EnumFlagsAttribute]
    public AllegianceFlags m_allegiance = AllegianceFlags.Team1;

    private Entity m_selectedAllyEntity = null;
    private GameObject m_selectedObject = null;
    private float m_inputTimer = 0.0f;
    private bool m_inputDetected = false;
    private Vector3 m_inputStartingPosition;
    private Vector3 m_inputCurrentPosition;
    
    private void Start()
    {
        GameLoader.CallAfterCompletion(this.Setup);
    }

    private void Setup()
    {
        GlobalTag.RunActionOnObject("player", this.SetupPlayerObject);
        GlobalTag.RegisterActionForObject("player", this.SetupPlayerObject);
    }

    private void OnDestroy()
    {
        GlobalTag.UnregisterActionForObject("player", this.SetupPlayerObject);
    }

    private void SetupPlayerObject(GameObject entity)
    {
    }

    private void PopulateBlackboard()
    {
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_inputDetected = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_inputDetected = false;
        }



        if (m_inputDetected)
        {
            m_inputTimer += Time.deltaTime;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            LayerMask layerMask  = ( 1 << LayerMask.NameToLayer("player"));
            //layerMask = ~layerMask;


            if (Physics.Raycast(ray, out hit, 1000.0f, layerMask))
            {
                if (m_selectedObject == null)
                {
                    m_selectedObject = hit.collider.gameObject;
                    m_inputStartingPosition = hit.point;
                    if (IsMyAlly(m_selectedObject))
                    {
                        m_selectedAllyEntity = Entity.Get(m_selectedObject.GetInstanceID());
                    }
                }

                m_inputCurrentPosition = hit.point;
                //Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
            }
        }
        else
        {
            m_selectedObject = null;
            m_inputTimer = 0.0f;
        }

        PopulateBlackboard();
    }

    private bool IsMyAlly(GameObject obj)
    {
        if (obj == null)
            return false;
        Entity entity = Entity.Get(obj.GetInstanceID());
        if (entity == null)
            return false;
        EntityCombat combatController = entity.GetEntityComponent<EntityCombat>();
        if (combatController == null)
            return false;

        return combatController.IsAlly(m_allegiance);
    }


    public void DrawLineBetweenStartAndCurrent()
    {
        Vector3 delta = m_inputCurrentPosition - m_inputStartingPosition;
        if (delta.sqrMagnitude > 1.0f)
        {
            Debug.DrawLine(m_inputStartingPosition, m_inputCurrentPosition, Color.red);
        }
    }

    public void DrawLineBetweenSelectedAndCurrent()
    {
        if (m_selectedObject == null)
        {
            return;
        }

        Vector3 startingPos = m_selectedObject.transform.position;
        Vector3 delta = m_inputCurrentPosition - startingPos;
        if (delta.sqrMagnitude > 1.0f)
        {
            Debug.DrawLine(startingPos, m_inputCurrentPosition, Color.red);
        }
    }

    public void DrawLineBetweenSelectedAllyAndCurrent()
    {
        if (IsMyAlly(m_selectedObject) == false)
        {
            return;
        }

        Vector3 startingPos = m_selectedObject.transform.position;
        Vector3 delta = m_inputCurrentPosition - startingPos;
        if (delta.sqrMagnitude > 1.0f)
        {
            Debug.DrawLine(startingPos, m_inputCurrentPosition, Color.red);
        }
    }

    public bool PlayableCharacterSelected()
    {
        return IsMyAlly(m_selectedObject);
    }

    public void MoveAllyToCurrentPosition()
    {
        // Passed the delta threshold
        if ((m_inputCurrentPosition - m_inputStartingPosition).sqrMagnitude < 1.0f)
            return;

        // Valid character selected
        if (m_selectedAllyEntity == null)
            return;

        // Is able to move
        EntityMovement movementController = m_selectedAllyEntity.GetEntityComponent<EntityMovement>();
        if (movementController == null)
        {
            DebugUtil.LogWarning("Selected entity does not have a movement controller");
            return;
        }

        // Move
        movementController.GoToPosition(m_inputCurrentPosition);
    }

    public void AttackTarget()
    {
        DebugUtil.Log("Attacking " + m_selectedObject.gameObject.name);
    }
}
