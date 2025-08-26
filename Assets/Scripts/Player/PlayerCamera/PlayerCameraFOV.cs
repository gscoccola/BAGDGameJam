using UnityEngine;

[RequireComponent(typeof(PlayerCameraController))]
public class PlayerCameraFOV : MonoBehaviour
{
    private bool _isOwner; //For netowking
    
    [Header("Sprint FOV")]
    [SerializeField, Range(0f, 100f)] private float sprintFOVBoostPercentage = 10f;
    private float _originalCameraFOV;
    private float _sprintCameraFOV;
    [SerializeField, Range(0f, 100f)] private float fovTransitionSpeed = 8f;
    private Camera _camera;
    private PlayerController _playerController;
    
    
    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        _originalCameraFOV = _camera.fieldOfView;
        _sprintCameraFOV = _originalCameraFOV * (1f + sprintFOVBoostPercentage / 100);
        _playerController = transform.parent.GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdateCameraFOV();
    }

    
    #region FOV Methods
    private void UpdateCameraFOV()
    {
        if (_playerController.CurrentSpeed >= _playerController.Preset.SprintSpeed)
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _sprintCameraFOV, fovTransitionSpeed * Time.deltaTime);
        }
        else
        {
            if (_camera.fieldOfView == _originalCameraFOV) return;
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _originalCameraFOV, fovTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(_camera.fieldOfView - _originalCameraFOV) < 0.01f) _camera.fieldOfView = _originalCameraFOV;
        }
    }
    #endregion
}