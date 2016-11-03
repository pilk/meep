using System;
using System.Collections;
using System.Collections.Generic;

namespace Rules
{
    public class RulesAffectFunctions
    {
        public delegate void AffectFunction(DataCenter.RuleDefinition rule, object source, object target);

        protected readonly Dictionary<string, AffectFunction> m_functionTable = new Dictionary<string, AffectFunction>(System.StringComparer.Ordinal);
        public Dictionary<string, AffectFunction> functionTable
        {
            get { return m_functionTable; }
        }

        public RulesAffectFunctions()
        {

        }

        public void ExecuteFunction(string functionName, DataCenter.RuleDefinition rule, object source, object target)
        {
            AffectFunction func = null;
            m_functionTable.TryGetValue(functionName, out func);
            if (func == null)
            {
                RulesSystem.Instance.Error("Could not find function " + functionName);
            }
            else
            {
                func.Invoke(rule, source, target);
            }
        }
    }
}