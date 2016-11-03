using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour 
{
    static private Dictionary<int, Entity> s_entityInstances = new Dictionary<int, Entity>(64);

    static public Entity Get(int ID)
    {
        Entity entity;
        s_entityInstances.TryGetValue(ID, out entity);
        return entity;
    }

    static public List<Entity> AllEntities()
    {
        return new List<Entity>(s_entityInstances.Values);
    }



    // Cached dictionary of all of our entity components for fast retrieval
    // There should never be more than one of the same type
    private Dictionary<System.Type, EntityComponent> m_entityComponentByType = new Dictionary<System.Type, EntityComponent>();
 


    private void Awake()
    {
        s_entityInstances.Add(this.gameObject.GetInstanceID(), this);
    }

    private void OnDestroy()
    {
        s_entityInstances.Remove(this.gameObject.GetInstanceID());
    }

    public void RegisterEntityComponent(EntityComponent component)
    {
        m_entityComponentByType.Add(component.GetType(), component);
    }

    public void UnregisterEntityComponent(EntityComponent component)
    {
        m_entityComponentByType.Remove(component.GetType());
    }

    public T GetEntityComponent<T>() where T : EntityComponent
    {
        if (m_entityComponentByType.ContainsKey(typeof(T)))
        {
            return (T)m_entityComponentByType[typeof(T)];
        }
        return null;
    }

    [ContextMenu("Quick Setup")]
    public void QuickSetup()
    {
        this.gameObject.GetOrAddMissingComponent<EntityMovement>();
        this.gameObject.GetOrAddMissingComponent<EntityCombat>();
    }










    public System.Action<Vector3> Move = null;
    public System.Action FinishedMoving = null;
}
