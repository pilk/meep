using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventCenter
{
    public delegate void EntityEventHandler(Entity entity);
    public EntityEventHandler PlayerSpawnedEvent = delegate { };
    public EntityEventHandler EntitySpawnedEvent = delegate { };
}
