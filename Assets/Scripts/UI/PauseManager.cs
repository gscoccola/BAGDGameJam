using System.Collections;
using PixelMate.Collectibles;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class PauseManager : MonoBehaviour
{
    GameObject _pauseCanvas;
    GameObject _deathCanvas;
    private bool _isPaused = false;
    private bool _isDead = false;
    [SerializeField] private LocalizeStringEvent _localizeStringEvent;


    [SerializeField] private CollectiblesManager collectiblesManager;
    [SerializeField] private TextMeshProUGUI collecionablesText;
    
    
    
    private void Awake()
    {
        _pauseCanvas = transform.GetChild(0).GetChild(1).gameObject;
        _deathCanvas = transform.GetChild(0).GetChild(2).gameObject;
        EventManager.Instance.Subscribe(Constants.OnPlayerDeath, OnPlayerDeath);
    }

    private void OnDestroy()
    {
        //EventManager.Instance.Unsubscribe(Constants.OnPlayerDeath, OnPlayerDeath);
    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
    }

    private void Start()
    {
        StartCoroutine(IStartDialog());
    }

    public void Pause()
    {
        if (_isDead) return;
        if (!_isPaused)
        {
            Time.timeScale = 0f;
            _pauseCanvas.SetActive(true);
            _isPaused = true;
            EventManager.Instance.Trigger(Constants.OnDisableControls);
        }
        else
        {
            Time.timeScale = 1f;
            _pauseCanvas.SetActive(false);
            _isPaused = false;
            EventManager.Instance.Trigger(Constants.OnEnableControls);
        }
    }

    private void OnPlayerDeath()
    {
        string currentCollecionablesText = $"{collectiblesManager.CollectedCount}/5";
        LocalizedString info = new LocalizedString("Eng-Esp", "COLECCIONABLES");
        info.Arguments = new object[] { new {TOTAL = currentCollecionablesText} };
        collecionablesText.text = info.GetLocalizedString();


        StartCoroutine(OnPlayerDeathCoroutine());
        _isDead = true;
        _deathCanvas.SetActive(true);
    }

    private IEnumerator OnPlayerDeathCoroutine()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;
    }

    public void LoadScene(int index)
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }

    private IEnumerator IStartDialog()
    {
        yield return new WaitForSeconds(5f);
        _localizeStringEvent.StringReference.TableEntryReference = "Dialog6";
        yield return new WaitForSeconds(5f);
        _localizeStringEvent.StringReference.TableEntryReference = "Dialog7";
        yield return new WaitForSeconds(5f);
        _localizeStringEvent.StringReference.TableEntryReference = "Dialog8";
        yield return new WaitForSeconds(5f);
        _localizeStringEvent.StringReference.TableEntryReference = "Empty";
    }
}
