using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    GameObject _pauseCanvas;
    private bool _isPaused = false;

    private void Awake()
    {
        _pauseCanvas = transform.GetChild(0).gameObject;
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

    public void LoadScene(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }
}
