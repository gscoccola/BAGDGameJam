using UnityEngine;

public class LocaleInfo : MonoBehaviour
{
    public string CurrentLocale { get; set; }
    public bool IsLocaleSet { get; set; } = false;
    public static LocaleInfo Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
}
}
