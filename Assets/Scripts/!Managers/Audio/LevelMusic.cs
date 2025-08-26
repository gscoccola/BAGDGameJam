using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class LevelMusic : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip[] tracks;
    private int _lastTrackIndex = -1;
    private AudioSource _audioSource;
    private const int MAX_RANDOM_ATTEMPTS = 10;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _lastTrackIndex = -1;
#if UNITY_EDITOR
        if (tracks.Length == 0)
        {
            Debug.LogWarning($"GAC: Any music are assigned on LevelMusic", this);
            return;
        }
#endif
        StartCoroutine(ChangeMusic());
        
        EventManager.Instance.Subscribe(Constants.MuteMusic, MuteMusic);
        EventManager.Instance.Subscribe(Constants.UnMuteMusic, UnMuteMusic);
    }

    
    #region Utilities
    private void MuteMusic()
    {
        _audioSource.mute = true;
    }

    private void UnMuteMusic()
    {
        _audioSource.mute = false;
    }
    #endregion


    #region Level Music
    private IEnumerator ChangeMusic()
    {
        _audioSource.Stop();
        _audioSource.PlayOneShot(tracks[GetRandomTrackIndex()]);
        
        yield return new WaitForSeconds(tracks[_lastTrackIndex].length);

        StartCoroutine(ChangeMusic());
    }

    private int GetRandomTrackIndex()
    {
        if (tracks.Length <= 1) return 0;

        int randomIndex = Random.Range(0, tracks.Length);
        for (int i = 0; i < MAX_RANDOM_ATTEMPTS; i++)
        {
            if (randomIndex != _lastTrackIndex)
            {
                _lastTrackIndex = randomIndex;
                return randomIndex;
            }
            randomIndex = Random.Range(0, tracks.Length);
        }
        return _lastTrackIndex;
    }
    #endregion
}