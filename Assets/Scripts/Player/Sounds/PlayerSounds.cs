using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class PlayerSounds : MonoBehaviour
{
    [Header("Footsteps")]
    [SerializeField, Range(0f, 0.25f)] private float volumeOffset = 0.2f;
    [SerializeField, Range(0f, 0.25f)] private float pitchOffset = 0.15f;
    [SerializeField] private SurfaceDetailsScriptable surfaceDetails;
    private bool _canPlaySounds = true;
    private PlayerController _playerController;
    private AudioSource _audioSource;

    [Header("CoolDowns")]
    [SerializeField, Range(0.1f, 2f)] private float walkCooldown = 0.6f;
    [SerializeField, Range(0.1f, 2f)] private float sprintCooldown = 0.4f;
    [SerializeField, Range(0.1f, 2f)] private float crouchCooldown = 0.8f;

    [Header("Jump")]
    [SerializeField] private CustomAudioClip[] jumpAudioClips;
    [SerializeField] private CustomAudioClip[] landAudioClips;
    
    
    private void Awake()
    {
#if UNITY_EDITOR
        if (!surfaceDetails) Debug.LogError($"GAC: Missing to assign a surfaceDetails", this);
#endif
        _playerController = transform.parent.GetComponent<PlayerController>();
        _playerController.OnMove += PlayFootsteps;
        _playerController.OnJump += PlayRandomJumpClip;
        _playerController.OnLand += PlayRandomLandClip;
        _audioSource = GetComponent<AudioSource>();
    }
    

    #region Footsteps
    private void PlayFootsteps()
    {
        //Si no se puede reproducir sonido, no hacemos nada
        if (!_canPlaySounds) return;

        CustomAudioClip[] footSteps = Array.Empty<CustomAudioClip>();
        string tag = "Default";
        float cooldown = 0;
        
        switch (_playerController.CurrentPlayerState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Walking:
                footSteps = surfaceDetails.SurfaceFootsteps(tag).walkFootsteps;
                cooldown = walkCooldown;
                break;
            case PlayerState.Sprinting:
                footSteps = surfaceDetails.SurfaceFootsteps(tag).sprintFootsteps;
                cooldown = sprintCooldown;
                break;
            case PlayerState.Crouching:
                footSteps = surfaceDetails.SurfaceFootsteps(tag).walkFootsteps;
                cooldown = crouchCooldown;
                break;
            case PlayerState.Falling:
                break;
            case PlayerState.Landed:
                break;
            case PlayerState.Recovery:
                break;
        }
        
        if (_playerController.haveASheep)
        {
            footSteps = surfaceDetails.SurfaceFootsteps(tag).sheepFootsteps;
        }
        
        if (footSteps.Length == 0) return;
        PlayRandomFootstepClipWithCooldown(footSteps, cooldown);
    }

    private void PlayRandomFootstepClipWithCooldown(CustomAudioClip[] footSteps, float cooldown)
    {
        PlayRandomClip(footSteps);
        StartCoroutine(FootstepCooldown(cooldown));
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

    private IEnumerator FootstepCooldown(float cooldown)
    {
        _canPlaySounds = false;
        yield return new WaitForSeconds(cooldown);
        _canPlaySounds = true;
    }
    #endregion


    #region Jump
    private void PlayRandomJumpClip()
    {
        if (jumpAudioClips.Length == 0) return;
        PlayRandomClip(jumpAudioClips);
    }
    
    private void PlayRandomLandClip()
    {
        if (landAudioClips.Length == 0) return;
        PlayRandomClip(landAudioClips);
    }
    #endregion
}