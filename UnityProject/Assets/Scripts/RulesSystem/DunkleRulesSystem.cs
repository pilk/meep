using UnityEngine;
using System.Collections;

using Rules;

public class DunkleRulesSystem : RulesSystem 
{
    public override void Initialize()
    {
        base.Initialize();
        m_rulesAffectFunctions = new DunkleRulesAffectfunctions();
        m_rulesConditionFunctions = new DunkleRulesConditionFunctions();
    }

    public override void Log(string text)
    {
        DebugUtil.Log(DebugFlags.RulesSystem, text);
    }

    public override void Error(string text)
    {
        DebugUtil.LogError(DebugFlags.RulesSystem, text);
    }
}