using UnityEngine;
using System.Collections;

public interface IAsyncLoadable
{
    IEnumerator AsyncStartup();
};

public abstract class GameLoader : MonoBehaviour
{
    static public void CallAfterCompletion(System.Action callback)
    {
        if (s_instance != null && s_instance.m_completed)
        {
            callback.Invoke();
        }
        else
        {
            s_gameLoadedEventHandler += callback;
        }

        if (s_instance == null)
        {
            GameObject.Instantiate(Resources.Load("GameLoader"));
        }
    }

    static protected event System.Action s_gameLoadedEventHandler = null;
    static protected GameLoader s_instance = null;

    private bool m_completed = false;
    private int m_loadedSceneIndex = -1;
    private Transform m_systemTransform = null;
    protected float m_percentageComplete = 0.0f;


    public bool loadedFromEntryScene
    {
#if UNITY_EDITOR
        get { return m_loadedSceneIndex == 0; }
#else
        get{ return false; }
#endif
    }

    static public bool isLoading
    {
        get { return s_instance != null && s_instance.m_completed == false; }
    }

    static public float percentageComplete
    {
        get { return s_instance.m_percentageComplete; }
    }


    private void Awake()
    {
        if (s_instance != null)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        s_instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
        this.StartCoroutine(Load());
    }

    /// <summary>
    /// Entry point for first time game load
    /// </summary>
    /// <returns></returns>
    private IEnumerator Load()
    {
        m_systemTransform = (new GameObject("GameSystems")).GetComponent<Transform>();
        DontDestroyOnLoad(m_systemTransform.gameObject);

        yield return StartCoroutine(LoadingSequence());

        m_loadedSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (s_gameLoadedEventHandler != null)
        {
            s_gameLoadedEventHandler.Invoke();
            s_gameLoadedEventHandler = null;
        }
    }

    /// <summary>
    /// Coroutine function provided by each game for unique game loading functionality
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator LoadingSequence();

    /// <summary>
    /// Takes in a generic class type to start up as a game system
    /// If the system happens to implement the IAsyncLoadable interface it will async load it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="systemName"></param>
    /// <returns></returns>
    protected IEnumerator GenericSystemStartup<T>(string systemName = null) where T : MonoBehaviour
    {
        T system = (new GameObject(typeof(T).Name)).AddComponent<T>();
        system.GetComponent<Transform>().parent = m_systemTransform;
        GameSystems.Register<T>(system);

        IAsyncLoadable asyncLoadable = system as IAsyncLoadable;
        if (asyncLoadable != null)
        {
            yield return Coroutines.Start(asyncLoadable.AsyncStartup());
        }
    }
}
