using UnityEngine;

public class PlayerCameraController : Singleton<PlayerCameraController>
{

    [Header("Player Camera")]
    [SerializeField, ReadOnly] private bool canMoveCamera = true;
    private int _canControllableCount;
    [SerializeField, ReadOnly] private Vector2 lookInput;
    [SerializeField, ReadOnly] private float currentVerticalAngle;
    [SerializeField, Range(0, 90f)] private float lowerVerticalClamp = 85f;
    [SerializeField, Range(0, 90f)] private float upperVerticalClamp = 85f;
    private Quaternion _targetPlayerRotation;
    private Quaternion _targetCameraRotation;
    private Camera _camera;
    public Camera GetMainCamera { get; private set; }
    private Vector3 _originalCameraLocalPosition;
    private Transform _playerControllerTf;
    private Quaternion _cameraRotationBeforeTransition;
    private Vector3 _cameraPositionBeforeTransition;
    [SerializeField] private bool _isCinematic;
    private float _currentHorizontalAngle;

    [Header("Effects")]
    [SerializeField, ReadOnly] private Vector3 cameraPositionOffset = Vector3.zero;


    protected override void Awake()
    {
        base.Awake();
        GetMainCamera = Camera.main;
        _camera = GetComponentInChildren<Camera>();
        _originalCameraLocalPosition = _camera.transform.localPosition;
        _targetPlayerRotation = transform.rotation;
        _playerControllerTf = transform.parent;
        _targetCameraRotation = _playerControllerTf.localRotation;
        EnableRotateCamera();
        EventManager.Instance.Subscribe(Constants.OnPlayerDeath, DisableRotateCamera);
        EventManager.Instance.Subscribe(Constants.OnEnableControls, EnableRotateCamera);
        EventManager.Instance.Subscribe(Constants.OnDisableControls, DisableRotateCamera);
    }

    protected override void OnDestroy()
    {
        //EventManager.Instance.Unsubscribe(Constants.OnPlayerDeath, DisableRotateCamera);
        //EventManager.Instance.Unsubscribe(Constants.OnEnableControls, EnableRotateCamera);
        //EventManager.Instance.Unsubscribe(Constants.OnDisableControls, DisableRotateCamera);
    }


    private void LateUpdate()
    {
        if (!canMoveCamera) return;
        
        lookInput = PlayerControls.Instance.MouseDelta();
        LookUpdate();
    }


    #region Camera Methods
    private void EnableRotateCamera()
    {
        _canControllableCount--;
        if (_canControllableCount < 0) _canControllableCount = 0;
        if (_canControllableCount != 0) return;
        
        canMoveCamera = true;
    }
    
    private void DisableRotateCamera()
    {
        _canControllableCount++;
        if (_canControllableCount > 1) return;

        canMoveCamera = false;
    }
    
    private void LookUpdate()
    {
        Vector2 input = new Vector2(lookInput.x, lookInput.y);
        
        currentVerticalAngle -= input.y;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -lowerVerticalClamp, upperVerticalClamp);


        if (_isCinematic)
        {
            _currentHorizontalAngle += input.x;
            _currentHorizontalAngle = Mathf.Clamp(_currentHorizontalAngle, -90, 90);
            _camera.transform.localRotation = Quaternion.Euler(currentVerticalAngle + Mathf.Sin(Time.time * 0.9f) * 10f, _currentHorizontalAngle, 0);
            //_playerControllerTf.Rotate(Vector3.up * horizontalAngle);
            _camera.transform.localPosition = _originalCameraLocalPosition + cameraPositionOffset;
            return;
        }

        _camera.transform.localRotation = Quaternion.Euler(currentVerticalAngle, 0, 0);
        _playerControllerTf.Rotate(Vector3.up * input.x);
        _camera.transform.localPosition = _originalCameraLocalPosition + cameraPositionOffset;
    }

    public void UpdateCameraPositionOffset(Vector3 cameraOffset)
    {
        cameraPositionOffset = cameraOffset;
    }
    #endregion
}