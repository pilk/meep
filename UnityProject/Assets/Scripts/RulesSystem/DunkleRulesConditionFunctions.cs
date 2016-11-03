using UnityEngine;
using System.Collections;

using Rules;
using DataCenter;

public class DunkleRulesConditionFunctions : Rules.RulesConditionFunctions 
{
    public DunkleRulesConditionFunctions()
        : base()
    {
        m_functionTable.Add("HasStatus", this.HasStatus);
    }

    public bool HasStatus(DataCenter.ConditionDefinition condition, object source, object target)
    {
        RulesComponent targetRules = RulesComponent.Get((GameObject)target);
        for (int i = targetRules.statusInstances.Count - 1; i >= 0; --i)
        {
            if( targetRules.statusInstances[i].statusDefinition.name.Equals(condition.value, System.StringComparison.Ordinal) )
            {
                return true;   
            }
        }
        return false;
    }
}
