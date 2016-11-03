using UnityEngine;
using System.Collections;

public class Coroutines : MonoBehaviour
{
    public delegate IEnumerator CoroutineFunc(System.Action action);

    static private Coroutines s_instance = null;

    static public Coroutines Instance { get { return s_instance; } }

    static public void Initialize()
    {
        if (s_instance != null)
        {
            return;
        }

        s_instance = new GameObject("Coroutines").AddComponent<Coroutines>();
        GameObject.DontDestroyOnLoad(s_instance.gameObject);
    }

    static public Coroutine Start(IEnumerator coroutine)
    {
        Initialize();
        return s_instance.StartCoroutine(coroutine);
    }

    static public void CallAfterTime(float time, System.Action action)
    {
        Start(_callAfterTime(time, action));
    }

    static private IEnumerator _callAfterTime(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        if (action != null)
        {
            action.Invoke();
        }
    }

    static public void DelayOneFrame(System.Action action)
    {
        Start(_delayOneFrame(action));
    }

    static private IEnumerator _delayOneFrame(System.Action action)
    {
        yield return new WaitForEndOfFrame();
        if (action != null)
        {
            action.Invoke();
        }
    }

    static public void DelayFrames(int frames, System.Action action)
    {
        Start(_delayFrames(frames, action));
    }

    static private IEnumerator _delayFrames(int frames, System.Action action)
    {
        for (int i = frames; i >= 0; --i)
        {
            yield return new WaitForEndOfFrame();
        }
        if (action != null)
        {
            action.Invoke();
        }
    }

    static public void Condition(System.Func<bool> condition, System.Action action)
    {
        Start(_condition(condition, action));
    }

    static private IEnumerator _condition(System.Func<bool> condition, System.Action action)
    {
        while (condition.Invoke() == false)
        {
            yield return null;
        }

        if (action != null)
        {
            action.Invoke();
        }
    }
}
