using System.Collections;
using UnityEngine;

public class PlayerPersecutionMusic : MonoBehaviour
{

    [SerializeField] private CustomAudioClip persecutionMusic;
    public AudioSource audioSource;
    [SerializeField, ReadOnly] private int count = 0;
    
    public float waitTimer = 5;
    private Coroutine _coroutine;
    private float _currentTimer;
    
    
    private void Awake()
    {
        count = 0;
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = persecutionMusic.clip;
        audioSource.loop = true;
        audioSource.volume = 0f;
        
        EventManager.Instance.Subscribe(Constants.OnEnemyChase, PlayMusic);
        EventManager.Instance.Subscribe(Constants.OnPEnemyEndChase, StopMusic);
        EventManager.Instance.Subscribe(Constants.OnPlayerDeath, OnPlayerDEad);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        //EventManager.Instance.Unsubscribe(Constants.OnEnemyChase, PlayMusic);
        //EventManager.Instance.Unsubscribe(Constants.OnPEnemyEndChase, StopMusic);
    }

    private void PlayMusic()
    {
        count++;
        
        if (_coroutine != null) return;
        audioSource.Play();
        StartCoroutine(FadeVolume(0f, persecutionMusic.volume, 1f));
    }

    private void StopMusic()
    {
        count--;
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(PersecutionCoroutine());
    }

    private IEnumerator PersecutionCoroutine()
    {
        _currentTimer = 0;
        while (_currentTimer < waitTimer)
        {
            _currentTimer += Time.deltaTime;

            // Cuando llega al 80% del tiempo -> empezar a bajar volumen
            float percent = _currentTimer / waitTimer;
            if (percent >= 0.8f)
            {
                float t = Mathf.InverseLerp(0.65f, 1f, percent); // 0 en 80%, 1 en 100%
                audioSource.volume = Mathf.Lerp(persecutionMusic.volume, 0f, t);
            }

            yield return null;
        }
        
        if (count == 0)
        {
            audioSource.Stop();
            _coroutine = null;
        }
    }
    
    private IEnumerator FadeVolume(float from, float to, float duration)
    {
        float time = 0f;
        audioSource.volume = from;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }

        audioSource.volume = to;
    }
    
    
    
    
    private void OnPlayerDEad()
    {
        StopAllCoroutines();
        audioSource.Stop();
    }
}