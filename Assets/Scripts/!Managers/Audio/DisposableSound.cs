using System.Collections;
using UnityEngine;

/// <summary>
/// Esta clase es el GameObject de cada instacia creada reproduciendo un sonido desde la clase AudioSystem.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DisposableSound : MonoBehaviour
{
    public AudioSource _audioSource;

    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    
    #region Audio
    /// <summary>
    ///Reproduce un AudioClip.
    /// </summary>
    /// <param name="audioClip"> el AudioClip a reproducir.</param>
    public void Play(AudioClip audioClip)
    {
        _audioSource.volume = 1;
        StartCoroutine(PlayClip(audioClip));
    }

    /// <summary>
    ///Reproduce un AudioClip junto a su custom volume.
    /// </summary>
    /// <param name="audioClip"> el AudioClip a reproducir.</param>
    /// <param name="volume"> el volume a reproducir.</param>
    public void Play(AudioClip audioClip, float volume)
    {
        _audioSource.volume = volume;
        StartCoroutine(PlayClip(audioClip));
    }
    
    /// <summary>
    ///Reproduce un AudioClip junto a su custom volume y un custom pitch.
    /// </summary>
    /// <param name="audioClip"> el AudioClip a reproducir.</param>
    /// <param name="volume"> el volume a reproducir.</param>
    /// <param name="pitch"> el pitch a reproducir.</param>
    public void Play(AudioClip audioClip, float volume, float pitch)
    {
        _audioSource.pitch = pitch;
        _audioSource.volume = volume;
        StartCoroutine(PlayClip(audioClip));
    }

    private IEnumerator PlayClip(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
        yield return new WaitForSeconds(clip.length);
        AudioSystem.Instance.AudioPool.ReturnObject(this);
    }
    #endregion


    #region Pool
    public static void TurnOn(DisposableSound disposableSound)
    {
        disposableSound.gameObject.SetActive(true);
    }

    public static void TurnOff(DisposableSound disposableSound)
    {
        disposableSound.transform.parent = AudioSystem.Instance.transform;
        disposableSound.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        AudioSystem.Instance.AudioPool.RemoveObject(this);
    }
    #endregion
}
