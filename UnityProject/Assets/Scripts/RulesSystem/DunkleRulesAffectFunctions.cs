using UnityEngine;
using System.Collections;

using Rules;
using DataCenter;

public class DunkleRulesAffectfunctions : Rules.RulesAffectFunctions
{
    public DunkleRulesAffectfunctions()
        : base()
    {
        m_functionTable.Add("DebugLog", this.DebugLog);
        m_functionTable.Add("DoDamage", this.DoDamage);
        m_functionTable.Add("AddStatus", this.AddStatus);
    }

    public void DebugLog(DataCenter.RuleDefinition rule, object source, object target)
    {
        DebugUtil.Log(rule.ruleDataTable.GetValue("text").value);
    }

    public void DoDamage(DataCenter.RuleDefinition rule, object source, object target)
    {
        RulesComponent targetGO = RulesComponent.Get((GameObject)target);
        targetGO.FireRulesEvent("on_take_damage", (GameObject)source, (GameObject)target);
    }
    

    public void AddStatus(DataCenter.RuleDefinition rule, object source, object target)
    {
        RulesComponent targetRules = RulesComponent.Get(target as GameObject);
        for( int i = 0, count = rule.statusList.Count;i < count; ++i )
        {
            targetRules.AddStatus(rule.statusList[i], (GameObject)source, (GameObject)target);
        }
    }
}