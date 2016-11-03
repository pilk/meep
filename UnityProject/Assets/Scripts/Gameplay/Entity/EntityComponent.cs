using UnityEngine;
using System.Collections;

public class EntityComponent : MonoBehaviour 
{
    protected Entity m_entityController = null;

    protected virtual void Awake()
    {
        if ((m_entityController = this.GetComponent<Entity>()) == null)
        {
            m_entityController = this.gameObject.GetComponentInParents<Entity>(true);
        }

        if (m_entityController == null)
        {
            DebugUtil.LogError("There is no Entity Component on " + this.gameObject.name);
            return;
        }

        m_entityController.RegisterEntityComponent(this);
    }

    protected virtual void Start()
    {
    }

    protected virtual void OnDestroy()
    {
        m_entityController.UnregisterEntityComponent(this);
    }
}
