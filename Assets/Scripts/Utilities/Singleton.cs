using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    
    
    protected virtual void Awake()
    {
        CreateSingleton();
    }
    
    
    private void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = GetComponent<T>();
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"GAC: Duplicated {GetType()} found", this);
#endif
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        // Always clear the static reference if this is the current instance
        if (Instance == this)
        {
            Instance = null;
        }
    }
}