using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Esta clase sirve para reproducir sonidos desde cualquier otra clase.
/// </summary>
public class AudioSystem : MonoBehaviour
{
    public static AudioSystem Instance { get; private set; }
    
    [Header("Audio System")]
    [SerializeField] private DisposableSound soundPrefab;
    public Pool<DisposableSound> AudioPool;
    [SerializeField] private int initialAmount = 10;
    [SerializeField, Range(0f, 0.25f)] private float volumeOffset = 0.2f;
    [SerializeField, Range(0f, 0.25f)] private float pitchOffset = 0.15f;
    private Transform _cameraTransform;
    
    
    private void Awake()
    {
        Singleton();
#if UNITY_EDITOR
        if (!soundPrefab) Debug.LogWarning($"GAC: Missing to assign a soundPrefab", this);
#endif
        AudioPool = new Pool<DisposableSound>(Factory, DisposableSound.TurnOn, DisposableSound.TurnOff, initialAmount);
        _cameraTransform = Camera.main != null ? Camera.main.transform : transform;
    }


    #region Utilities
    private void Singleton()
    {
        if (Instance == null) Instance = this;
        else
        {
#if UNITY_EDITOR
            Debug.LogError($"GAC: Duplicated {GetType()} found", this);
#endif
            Destroy(gameObject);
        }
    }
    #endregion


    #region AudioSystem
    /// <summary>
    /// GAC: Reproduce un clip con su sonido personalizado sobre la main camera.
    /// </summary>
    /// <param name="customAudioClip"> el clip junto al volumen a reproducir.</param>
    public void PlaySound(AudioClip customAudioClip)
    {
        DisposableSound disposableSound = AudioPool.GetObject();
        disposableSound.transform.position = _cameraTransform.position;
        disposableSound.transform.SetParent(_cameraTransform);
        disposableSound.Play(customAudioClip, 1);
    }
    
    
    /// <summary>
    /// GAC: Reproduce un clip con su sonido personalizado sobre la main camera.
    /// </summary>
    /// <param name="customAudioClip"> el clip junto al volumen a reproducir.</param>
    public void PlaySound(CustomAudioClip customAudioClip)
    {
        DisposableSound disposableSound = AudioPool.GetObject();
        disposableSound.transform.position = _cameraTransform.position;
        disposableSound.transform.SetParent(_cameraTransform);
        disposableSound.Play(customAudioClip.clip, customAudioClip.volume);
    }
    
    /// <summary>
    /// GAC: Reproduce un clip con su sonido personalizado en la posicion deseada.
    /// </summary>
    /// <param name="customAudioClip"> el clip junto al volumen a reproducir.</param>
    /// <param name="position">La posicion donde se va a reproducir el sonido.</param>
    public void PlaySound(CustomAudioClip customAudioClip, Vector3 position)
    {
        DisposableSound disposableSound = AudioPool.GetObject();
        disposableSound.transform.position = position;
        disposableSound.Play(customAudioClip.clip, customAudioClip.volume);
    }
    
    /// <summary>
    /// GAC: Reproduce un clip con su sonido personalizado en un transform en movimiento deseado.
    /// </summary>
    /// <param name="customAudioClip"> el clip junto al volumen a reproducir.</param>
    /// <param name="position">La posicion donde se va a reproducir el sonido.</param>
    /// <param name="parent">El transform que va a seguir el sonido.</param>
    public void PlaySound(CustomAudioClip customAudioClip, Vector3 position, Transform parent)
    {
        DisposableSound disposableSound = AudioPool.GetObject();
        disposableSound.transform.position = position;
        disposableSound.transform.SetParent(parent);
        disposableSound.Play(customAudioClip.clip, customAudioClip.volume);
    }
    
    /// <summary>
    /// GAC: Reproduce un clip con su sonido personalizado en la posicion deseada.
    /// GAC: También agrega un offset de volumen y un offset de pitch.
    /// </summary>
    /// <param name="customAudioClip"> el clip junto al volumen a reproducir.</param>
    /// <param name="position">La posicion donde se va a reproducir el sonido.</param>
    /// <param name="parent">El parent del sonido.</param>
    /// /// <param name="enableVolumeOffset">Si se activa el offset de volumen.</param>
    /// /// <param name="enablePitchOffset">Si se activa el offset de pitch.</param>
    public void PlaySound(CustomAudioClip customAudioClip, Vector3 position, Transform parent, bool enableVolumeOffset, bool enablePitchOffset)
    {
        DisposableSound disposableSound = AudioPool.GetObject();
        disposableSound.transform.position = position;
        disposableSound.transform.SetParent(parent);
        float customVolume = enableVolumeOffset ? customAudioClip.volume + Random.Range(-volumeOffset, 0) : customAudioClip.volume;
        float pitch = enablePitchOffset ? Random.Range(1 - pitchOffset, 1 + pitchOffset) : 1;
        disposableSound.Play(customAudioClip.clip, customVolume, pitch);
    }
    
    /// <summary>
    /// GAC: Este método se utiliza para indicar como se van a crear nuevas instancias de DisposableSound que se agregarán al pool.
    /// </summary>
    private DisposableSound Factory()
    {
        return Instantiate(soundPrefab, transform);
    }
    #endregion
}

/// <summary>
/// GAC: Esta clase sirve para almacenar un clip de audio junto a un volumen regulable.
/// </summary>
[Serializable] public class CustomAudioClip
{
    public AudioClip clip;
    [Range(0.1f, 1f)] public float volume = 1;
}