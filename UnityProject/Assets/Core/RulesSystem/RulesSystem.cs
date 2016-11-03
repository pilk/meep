using System;
using System.Collections;
using System.Collections.Generic;

namespace Rules
{
    abstract public class RulesSystem
    {
        protected RulesAffectFunctions m_rulesAffectFunctions = null;
        protected RulesConditionFunctions m_rulesConditionFunctions = null;

        static protected RulesSystem s_instance = null;
        static public RulesSystem Instance
        {
            get { return s_instance; }
        }

        //----------------------------------------------------------------------------------------------------

        public virtual void Initialize()
        {
            if (s_instance != null)
            {
                s_instance.Error("RulesSystem attempting to initialize more than once");
                return;
            }

            s_instance = this;
            Log("RulesSystem initialized");
        }

        //----------------------------------------------------------------------------------------------------

        public enum RuleParameters
        {
            SOURCE, TARGET, RULE, NUM_PARAMETERS
        }

        public void ExecuteRule(DataCenter.RuleDefinition rule, object source, object target)
        {
            // Check if we pass the conditions to run this rule
            if (ConditionsPassed(rule.conditionList, source, target) == false)
            {
                return;
            }

            // Run the rule
            if (!string.IsNullOrEmpty(rule.function))
            {
                m_rulesAffectFunctions.ExecuteFunction(rule.function, rule, source, target);
            }

            // Run any child rules
            for (int i = 0, count = rule.rulesList.Count; i < count; i++)
            {
                ExecuteRule(rule.rulesList[i], source, target);
            }
        }

        public void ExecuteRulesList(List<DataCenter.RuleDefinition> rulesList, object source, object target)
        {
            for (int i = 0, count = rulesList.Count; i < count; i++)
            {
                ExecuteRule(rulesList[i], source, target);
            }
        }

        public void ExecuteEvent(string eventName, List<DataCenter.EventDefinition> eventList, object source, object target)
        {
            for (int i = 0, count = eventList.Count; i < count; i++)
            {
                if (eventList[i].eventName.Equals(eventName, StringComparison.Ordinal))
                {
                    ExecuteRulesList(eventList[i].rulesList, source, target);
                }
            }
        }

        public bool ConditionsPassed(List<DataCenter.ConditionDefinition> conditionList, object source, object target)
        {
            if (conditionList.Count == 0) return true;

            // Execute the children first
            for (int i = 0, count = conditionList.Count; i < count; ++i)
            {
                if (ConditionsPassed(conditionList[i].conditionList, source, target) == false)
                {
                    return false;
                }
            }

            // Evaluate all the conditions on this level
            int orIndex = 0;
            List<List<DataCenter.ConditionDefinition>> conditionsIndexedByOr = new List<List<DataCenter.ConditionDefinition>>();
            conditionsIndexedByOr.Add(new List<DataCenter.ConditionDefinition>());
            for (int i = 0, count = conditionList.Count; i < count; ++i)
            {
                if (conditionList[i].type == DataCenter.ConditionDefinition.ConditionType.OR)
                {
                    ++orIndex;
                    conditionsIndexedByOr.Add(new List<DataCenter.ConditionDefinition>());
                }

                conditionsIndexedByOr[orIndex].Add(conditionList[i]);
            }

            foreach (List<DataCenter.ConditionDefinition> conditions in conditionsIndexedByOr)
            {
                bool success = true;
                for (int i = 0, count = conditions.Count; i < count; ++i)
                {
                    DataCenter.ConditionDefinition condition = conditions[i];
                    if (m_rulesConditionFunctions.InvokeCondition(condition.function, condition, source, target) != condition.result)
                    {
                        // We failed a condition
                        success = false;
                        break;
                    }
                }

                if (success)
                {
                    return true;
                }
            }

            return false;
        }

        abstract public void Log(string text);
        abstract public void Error(string text);
    };
}