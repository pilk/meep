using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour 
{
    public string m_objectDefinitionName = null;
    public bool m_autoSpawn = false;

    private void Start()
    {
        if (m_autoSpawn)
        {
            this.Spawn();
        }
    }

    public void Spawn()
    {
        ObjectDefinition objectDefinition;
        if (GameSystems.Get<DataCenter.DataCenterManager>().TryGetByName(m_objectDefinitionName, out objectDefinition))
        {
            GameObject newObject = new GameObject();
            Transform t = newObject.transform;
            t.position = this.transform.position;
            t.eulerAngles = this.transform.eulerAngles;
            objectDefinition.Setup(newObject);
        }
    }
}
