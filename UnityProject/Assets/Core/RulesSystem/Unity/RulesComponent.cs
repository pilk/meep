using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Unity-game-side-glue code for rules system
public class RulesComponent : MonoBehaviour
{
    protected Rules.RulesMixin m_rulesMixin = new Rules.RulesMixin();

    static protected Dictionary<int, RulesComponent> m_rulesComponentInstances = new Dictionary<int, RulesComponent>();

    public List<Rules.StatusInstance> statusInstances
    {
        get { return m_rulesMixin.m_statusInstanceList; }
    }
    
    //--------------------------------------------------
    static public RulesComponent Get(GameObject obj)
    {
        return RulesComponent.Get(obj.GetInstanceID());
    }

    //--------------------------------------------------
    static public RulesComponent Get(int ID)
    {
        RulesComponent ret = null;
        m_rulesComponentInstances.TryGetValue(ID, out ret);
        return ret;
    }

    //--------------------------------------------------
    private void Awake()
    {
        m_rulesComponentInstances.Add(this.gameObject.GetInstanceID(), this);
    }

    //--------------------------------------------------
    private void OnDestroy()
    {
        m_rulesComponentInstances.Remove(this.gameObject.GetInstanceID());
    }

    //--------------------------------------------------
    private void Update()
    {
        m_rulesMixin.Update(Time.deltaTime);
    }

    //--------------------------------------------------
    public void AddStatus(DataCenter.StatusDefinition status, GameObject source, GameObject target)
    {
        Rules.RulesSystem.Instance.Log("Adding status [" + status.name + "] to target [" + target.name + "]");
        m_rulesMixin.AddStatus(status, source, target);
    }

    //--------------------------------------------------
    public void FireRulesEvent(string eventName, GameObject source, GameObject target)
    {
        // Send an event to any status instances currently affecting this target
        m_rulesMixin.OnEvent(eventName, source, target);
    }
}
