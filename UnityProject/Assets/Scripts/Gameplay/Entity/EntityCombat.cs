using UnityEngine;
using System.Collections;

public class EntityCombat : EntityComponent
{
    [SerializeField]
    [EnumFlagsAttribute]
    public AllegianceFlags m_allegiance = AllegianceFlags.Team1;

    public bool IsAlly(AllegianceFlags allegiance)
    {
        return m_allegiance == allegiance;
    }
}
