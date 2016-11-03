using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;

namespace Rules
{
    public class RulesConditionFunctions
    {
        public delegate bool ConditionFunction(DataCenter.ConditionDefinition condition, object source, object target);

        protected readonly Dictionary<string, ConditionFunction> m_functionTable = new Dictionary<string, ConditionFunction>(System.StringComparer.Ordinal);
        public Dictionary<string, ConditionFunction> functionTable
        {
            get { return m_functionTable; }
        }

        public RulesConditionFunctions()
        {

        }

        public bool InvokeCondition(string functionName, DataCenter.ConditionDefinition condition, object source, object target)
        {
            ConditionFunction func = null;
            m_functionTable.TryGetValue(functionName, out func);
            if (func == null)
            {
                RulesSystem.Instance.Error("Could not find condition " + functionName);
            }
            else
            {
                return func.Invoke(condition, source, target);
            }
            return false;
        }

    }
}