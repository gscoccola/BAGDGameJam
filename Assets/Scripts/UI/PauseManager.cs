using System.Collections;
using PixelMate.Collectibles;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class PauseManager : Singleton<PauseManager>
{
    GameObject _pauseCanvas;
    GameObject _pauseCanvasDefaultButton;
    GameObject _deathCanvas;
    GameObject _deathCanvasDefaultButton;
    private bool _isPaused = false;
    private bool _isDead = false;
    float _buttonTimer = 1f;
    [SerializeField] public LocalizeStringEvent _localizeStringEvent;


    [SerializeField] private CollectiblesManager collectiblesManager;
    [SerializeField] private TextMeshProUGUI collecionablesText;

    [Header("Audio")]
    [SerializeField]
    private AudioClip _dialog2;
    [SerializeField]
    private AudioClip _dialog3;
    [SerializeField]
    private AudioClip _dialog4;
    [SerializeField]
    private AudioClip _dialog5;



    private new void Awake()
    {
        base.Awake();
        _pauseCanvas = transform.GetChild(0).GetChild(1).gameObject;
        _pauseCanvasDefaultButton = _pauseCanvas.transform.GetChild(2).gameObject;
        _deathCanvas = transform.GetChild(0).GetChild(2).gameObject;
        _deathCanvasDefaultButton = _deathCanvas.transform.GetChild(3).gameObject;
        EventManager.Instance.Subscribe(Constants.OnPlayerDeath, OnPlayerDeath);
    }

    private void Update()
    {
        _buttonTimer -= Time.unscaledDeltaTime;
    }

    private new void OnDestroy()
    {
        //EventManager.Instance.Unsubscribe(Constants.OnPlayerDeath, OnPlayerDeath);
    }

    private void Start()
    {
        StartCoroutine(IStartDialog());
    }

    public void Pause()
    {
        if (_isDead || _buttonTimer > 0f) return;
        _buttonTimer = 1f;
        if (!_isPaused)
        {
            Time.timeScale = 0f;
            _pauseCanvas.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_pauseCanvasDefaultButton);
            _isPaused = true;
            EventManager.Instance.Trigger(Constants.OnDisableControls);
        }
        else
        {
            Time.timeScale = 1f;
            _pauseCanvas.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
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
        EventSystem.current.SetSelectedGameObject(_deathCanvasDefaultButton);
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
        AudioSystem.Instance.PlaySound(_dialog2);
        yield return new WaitForSeconds(5f);
        _localizeStringEvent.StringReference.TableEntryReference = "Dialog6";
        AudioSystem.Instance.PlaySound(_dialog3);
        yield return new WaitForSeconds(5f);
        _localizeStringEvent.StringReference.TableEntryReference = "Dialog7";
        AudioSystem.Instance.PlaySound(_dialog4);
        yield return new WaitForSeconds(5f);
        _localizeStringEvent.StringReference.TableEntryReference = "Dialog8";
        AudioSystem.Instance.PlaySound(_dialog5);
        yield return new WaitForSeconds(5f);
        _localizeStringEvent.StringReference.TableEntryReference = "Empty";
    }


}
