using UnityEngine;
using System.Collections;

using DataCenter;

public class RulesTestObject : MonoBehaviour 
{
    RulesComponent zombie = null;
    RulesComponent victim = null;

    private void Start()
    {
        GameLoader.CallAfterCompletion(() =>{
            zombie = (new GameObject("rules1")).AddComponent<RulesComponent>();
            victim = (new GameObject("rules2")).AddComponent<RulesComponent>();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Create victim
            StatusDefinition victimStatus;
            DataCenter.DataCenterManager dataCenter = GameSystems.Get<DataCenter.DataCenterManager>();
            dataCenter.TryGetByName<DataCenter.StatusDefinition>("victim", out victimStatus);
            victim.AddStatus(victimStatus, victim.gameObject, victim.gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Create zombie
            StatusDefinition zombieStatus;
            DataCenter.DataCenterManager dataCenter = GameSystems.Get<DataCenter.DataCenterManager>();
            dataCenter.TryGetByName<DataCenter.StatusDefinition>("zombie", out zombieStatus);
            zombie.AddStatus(zombieStatus, zombie.gameObject, zombie.gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Hurt victim
            zombie.FireRulesEvent("on_cause_damage", zombie.gameObject, victim.gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // Kill victim
            zombie.FireRulesEvent("on_kill", zombie.gameObject, victim.gameObject);
            victim.FireRulesEvent("on_death", zombie.gameObject, victim.gameObject);
        }
    }
}
