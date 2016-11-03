using UnityEngine;
using DataCenter;

namespace DataCenter
{
    abstract public partial class DataObjectTemplate
    {
        public virtual void Setup(GameObject target)
        {

        }
    }
}

public partial class ObjectDefinition : DataObjectTemplate
{
    public override void Setup(GameObject target)
    {
        base.Setup(target);

        //target.name = this.objectName;
        target.name = this.name;

        //GameObject modelPrefab = Resources.Load<GameObject>("Models/" + this.model);
        //GameObject modelObject = GameObject.Instantiate(modelPrefab) as GameObject;
        //modelObject.name = this.model;
        //modelObject.transform.parent = target.transform;
        //modelObject.transform.localPosition = Vector3.zero;
        //modelObject.transform.localEulerAngles = Vector3.zero;
        //modelObject.AddComponent<EntityAnimation>();
    }
};

public partial class CharacterDefinition : ObjectDefinition
{
    public override void Setup(GameObject target)
    {
        base.Setup(target);

        Entity entity = Entity.Get(target.GetInstanceID());
        if (entity == null)
        {
            DebugUtil.LogError("Missing entity component on " + target.name);
            return;
        }

        entity.GetEntityComponent<EntityMovement>().m_speed = this.speed;
        entity.GetEntityComponent<EntityCombat>().m_allegiance = this.allegiance;
    }
}