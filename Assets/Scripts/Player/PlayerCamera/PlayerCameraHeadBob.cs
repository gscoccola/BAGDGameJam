using System;
using UnityEngine;

[RequireComponent(typeof(PlayerCameraController))]
public class PlayerCameraHeadBob : MonoBehaviour
{
    private HeadBobPreset _currentHeadBobPresetLerped;
    private HeadBobPreset _targetHeadBobPreset;
    private float _headBobTimer;
    private Vector3 _cameraPositionOffset;
    private Transform _cameraHeadBobTf;
    private PlayerController _playerController;
    
    
    private void Awake()
    {
        _cameraHeadBobTf = transform.GetChild(0);
        _playerController = GetComponentInParent<PlayerController>();
        _playerController.OnPlayerChangeState += UpdateCurrentHeadBobPreset;
    }
    
    private void Update()
    {
        UpdateHeadBobPresetLerp();
        UpdateHeadBobbing();
        StopHeadBob();
    }

    
    #region Head Bob Methods
    private void UpdateCurrentHeadBobPreset(PlayerState state)
    {
        switch(state)
        {
            case PlayerState.Walking:
                _targetHeadBobPreset = _playerController.Preset.WalkHeadBobPreset;
                break;
            case PlayerState.Sprinting:
                _targetHeadBobPreset = _playerController.Preset.SprintHeadBobPreset;
                break;
            case PlayerState.Crouching:
                _targetHeadBobPreset = _playerController.Preset.CrouchHeadBobPreset;
                break;
            default:
                _targetHeadBobPreset = new HeadBobPreset();
                break;
        }
    }
    
    private void UpdateHeadBobPresetLerp()
    {
        _currentHeadBobPresetLerped = HeadBobPreset.Lerp(_currentHeadBobPresetLerped, _targetHeadBobPreset, Time.deltaTime);
    }
    
    private void UpdateHeadBobbing()
    {
        if (PlayerControls.Instance.InputMovement().magnitude == 0)
        {
            _headBobTimer = 0f;
            return;
        }
        
        _headBobTimer += Time.deltaTime * _currentHeadBobPresetLerped.Frequency;
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Lerp(pos.y, Mathf.Sin(_headBobTimer) * _currentHeadBobPresetLerped.Amplitude * 1.4f, _currentHeadBobPresetLerped.Smooth * Time.deltaTime);
        pos.x = Mathf.Lerp(pos.x, Mathf.Cos(_headBobTimer / 2f) * _currentHeadBobPresetLerped.Amplitude * 1.6f, _currentHeadBobPresetLerped.Smooth * Time.deltaTime);
        _cameraHeadBobTf.localPosition += pos;
    }

    private void StopHeadBob()
    {
        if (_cameraHeadBobTf.localPosition == Vector3.zero) return;
        _cameraHeadBobTf.localPosition = Vector3.Lerp(_cameraHeadBobTf.localPosition, Vector3.zero, Time.deltaTime);
    }
    #endregion
}

[Serializable]
public struct HeadBobPreset
{
    [SerializeField, Range(0f, 0.01f)] private float amplitude;
    public float Amplitude => amplitude;
    [SerializeField, Range(0f, 50f)] private float frequency;
    public float Frequency => frequency;
    [SerializeField, Range(10f, 100f)] private float smooth;
    public float Smooth => smooth;
    
    public HeadBobPreset(float amplitude, float frequency, float smooth)
    {
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.smooth = smooth;
    }
    
    public static HeadBobPreset Lerp(HeadBobPreset from, HeadBobPreset to, float t)
    {
        return new HeadBobPreset
        (
            Mathf.Lerp(from.amplitude, to.amplitude, t),
            Mathf.Lerp(from.frequency, to.frequency, t),
            Mathf.Lerp(from.smooth, to.smooth, t)
        );
    }
}