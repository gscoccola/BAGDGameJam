using System.Collections;
using UnityEngine;

public class PlayerPersecutionMusic : MonoBehaviour
{

    [SerializeField] private CustomAudioClip persecutionMusic;
    public AudioSource audioSource;
    [SerializeField, ReadOnly] private int count = 0;
    public float waitTimer = 5;
    private Coroutine _coroutine;
    
    
    private void Awake()
    {
        count = 0;
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = persecutionMusic.clip;
        audioSource.loop = true;
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
    }

    private void StopMusic()
    {
        count--;
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(Coroutine());
    }

    private IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(waitTimer);
        if (count == 0)
        {
            audioSource.Stop();
            _coroutine = null;
        }
    }
    
    private void OnPlayerDEad()
    {
        StopAllCoroutines();
        audioSource.Stop();
    }
}