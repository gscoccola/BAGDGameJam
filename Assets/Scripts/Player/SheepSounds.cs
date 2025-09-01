using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SheepSounds : MonoBehaviour
{
    [Header("Footsteps")]
    [SerializeField, Range(0f, 0.25f)] private float volumeOffset = 0.2f;
    [SerializeField, Range(0f, 0.25f)] private float pitchOffset = 0.15f;
    private PlayerController _playerController;
    private AudioSource _audioSource;

    [Header("CoolDowns")]
    [SerializeField, Range(0.1f, 20f)] private float minCooldown = 8f;
    [SerializeField, Range(0.1f, 20f)] private float maxCooldown = 15f;
    private float _currentCooldown, _currentTimer;

    [Header("Sheep")]
    [SerializeField] private CustomAudioClip[] sheepSounds;

    private void Awake()
    {
        _playerController = transform.parent.GetComponent<PlayerController>();
        _audioSource = GetComponent<AudioSource>();
        _currentCooldown = Random.Range(minCooldown, maxCooldown);
    }

    private void Update()
    {
        if (_playerController.haveASheep != gameObject) return;
        

        float multiplier = 1;
        if (_playerController.CurrentPlayerState == PlayerState.Sprinting) multiplier = 1.33f;
        _currentTimer += Time.deltaTime * multiplier;
        if (sheepSounds.Length == 0) return;
        if (_currentTimer < _currentCooldown) return;
   
        _currentTimer = 0;
        _currentCooldown = Random.Range(minCooldown, maxCooldown);
        PlayRandomClip(sheepSounds);
        EventManager.Instance.Trigger(Constants.OnSheepNoise);
    }


    private void PlayRandomClip(CustomAudioClip[] clips)
    {
        int index = Random.Range(0, clips.Length);
        float volume = clips[index].volume + Random.Range(-volumeOffset, 0);
        volume = Mathf.Clamp(volume, 0.1f, 1f);
        _audioSource.pitch = Random.Range(1f - pitchOffset, 1f + pitchOffset);
        _audioSource.pitch = Mathf.Clamp(_audioSource.pitch, 0.1f, 3f);
        _audioSource.PlayOneShot(clips[index].clip, volume);
    }
    
}