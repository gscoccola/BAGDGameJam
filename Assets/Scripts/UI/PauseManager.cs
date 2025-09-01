using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    GameObject _pauseCanvas;
    GameObject _deathCanvas;
    private bool _isPaused = false;
    private bool _isDead = false;

    private void Awake()
    {
        _pauseCanvas = transform.GetChild(0).gameObject;
        _deathCanvas = transform.GetChild(1).gameObject;
        EventManager.Instance.Subscribe(Constants.OnPlayerDeath, OnPlayerDeath);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe(Constants.OnPlayerDeath, OnPlayerDeath);
    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
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
        Time.timeScale = 0f;
        _isDead = true;
        _deathCanvas.SetActive(true);
    }

    public void LoadScene(int index)
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }
}
