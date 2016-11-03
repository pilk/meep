using UnityEngine;
using System.Collections;

public static class GameObjectExtensions
{
    public static T GetOrAddMissingComponent<T>(this GameObject target) where T : MonoBehaviour
    {
        T ret = target.GetComponent<T>();
        if (ret == null)
            ret = target.AddComponent<T>();
        return ret;
    }

    public static T GetComponentInParents<T>(this GameObject target, bool includeThisLevel = false) where T : MonoBehaviour
    {
        T ret = null;
        if (includeThisLevel)
        {
            ret = target.GetComponent<T>();
            if (ret != null)
                return ret;
        }
        if (target.transform.parent == null)
            return null;
        return GetComponentInParents<T>(target.transform.parent.gameObject, true);
    }
}
