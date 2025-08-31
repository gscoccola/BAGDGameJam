using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public bool haveASheep;
    
    [Header("Controller")]
    [SerializeField] private PlayerPreset preset;
    public PlayerPreset Preset => preset;
    private Transform _playerCameraTransform;
    private Coroutine _crouchCoroutine;
    public PlayerSprintEnergy GetPlayerSprintEnergy { get; private set; }
    
    [Header("Movement")]
    [SerializeField, ReadOnly] private bool canControllable = true;
    public bool GetCanControllable => canControllable;
    private int _canControllableCount;
    [SerializeField, ReadOnly] private bool isGrounded = true;
    public bool GetIsGrounded => isGrounded;
    [SerializeField, ReadOnly] private bool isSprinting;
    public bool GetIsSprinting => isSprinting;
    [SerializeField, ReadOnly] private bool isCrouching;
    public bool GetIsCrouching => isCrouching;
    [SerializeField, ReadOnly] private PlayerState currentPlayerState = PlayerState.Idle;
    public PlayerState CurrentPlayerState { 
        get => currentPlayerState;
        set { 
            currentPlayerState = value;
            OnPlayerChangeState?.Invoke(currentPlayerState);
        }
    }
    public event Action<PlayerState> OnPlayerChangeState;
    [SerializeField, ReadOnly] private float currentSpeed;
    public float CurrentSpeed => currentSpeed;
    public bool IsMovingBackward => _moveInput.y < 0;
    private const float AntiBumpFactor = -2;
    [Min(0)] private float _currentCoyoteTimer;
    [Min(0)] private float _currentJumpBufferTimer;
    private Vector2 _moveInput;
    private Vector3 _targetVelocity;
    private Vector3 _currentMoveVelocity;
    private Vector3 _currentGravityVelocity;
    public Vector3 CurrentGravityVelocity => _currentGravityVelocity;
    public FiniteStateMachine<PlayerState> Fsm;
    private CharacterController _characterController;
    public PlayerSounds PlayerSounds { private set; get; }
    public event Action OnMove, OnJump, OnLand;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerCameraTransform = GetComponentInChildren<PlayerCameraController>().transform;
        _characterController.height = Preset.NormalHeight;
        _characterController.center = new Vector3(0, Preset.NormalHeight / 2, 0);
        Vector3 camStartPos = _playerCameraTransform.localPosition;
        Vector3 camTargetPos = new Vector3(camStartPos.x, Preset.NormalCameraHeight, camStartPos.z);
        _playerCameraTransform.localPosition = camTargetPos;
        GetPlayerSprintEnergy = GetComponent<PlayerSprintEnergy>();
        PlayerSounds = GetComponentInChildren<PlayerSounds>();
        
        EnableLocomotion();
        PlayerControls.Instance.OnSprintKey += StartPressSprint;
        PlayerControls.Instance.OnSprintKeyCancelled += StopPressSprint;
        PlayerControls.Instance.OnCrouchKey += PressCrouch;
        PlayerControls.Instance.OnJumpKey += RequestJump;
        
        Fsm = new FiniteStateMachine<PlayerState>();
        Fsm.AddState(PlayerState.Idle, new PlayerIdle(Fsm, this));
        Fsm.AddState(PlayerState.Walking, new PlayerWalk(Fsm, this));
        Fsm.AddState(PlayerState.Sprinting, new PlayerSprint(Fsm, this));
        Fsm.AddState(PlayerState.Crouching, new PlayerCrouch(Fsm, this));
        Fsm.AddState(PlayerState.Falling, new PlayerFall(Fsm, this));
        Fsm.AddState(PlayerState.Landed, new PlayerLanded(Fsm, this));
        Fsm.AddState(PlayerState.Recovery, new PlayerRecovery(Fsm, this));
        Fsm.ChangeState(PlayerState.Idle);
        EventManager.Instance.Subscribe(Constants.OnPlayerDeath, DisableLocomotion);
        EventManager.Instance.Subscribe(Constants.OnEnableControls, EnableLocomotion);
        EventManager.Instance.Subscribe(Constants.OnDisableControls, DisableLocomotion);
    }
    
    private void OnDestroy()
    {
        PlayerControls.Instance.OnSprintKey -= StartPressSprint;
        PlayerControls.Instance.OnSprintKeyCancelled -= StopPressSprint;
        PlayerControls.Instance.OnCrouchKey -= PressCrouch;
        PlayerControls.Instance.OnJumpKey -= RequestJump;
        EventManager.Instance.Unsubscribe(Constants.OnPlayerDeath, DisableLocomotion);
        EventManager.Instance.Unsubscribe(Constants.OnEnableControls, EnableLocomotion);
        EventManager.Instance.Unsubscribe(Constants.OnDisableControls, DisableLocomotion);
    }

    private void Update()
    {
        TryJump();
        Fsm.Update();
    }
    
    public void ActiveLandEvent() { OnLand?.Invoke(); }

    
    #region Inputs Methods
    private void StartPressSprint()
    {
        isSprinting = true;
    }
    
    private void StopPressSprint()
    {
        isSprinting = false;
    }
    
    private void PressCrouch()
    {
        if (!GetCanControllable || !GetIsGrounded || GetIsSprinting) return;
        
        if (!isCrouching) StartCrouch();
        else StopCrouch();
    }

    private void StartCrouch()
    {
        isCrouching = true;
        if (_crouchCoroutine != null) StopCoroutine(_crouchCoroutine);
        _crouchCoroutine = StartCoroutine(ChangeCrouchStateCoroutine(Preset.CrouchHeight, Preset.CrouchCameraHeight));
    }
    
    public bool StopCrouch()
    {
        if (!GetIsCrouching || !CheckCanEndCrouch()) return false;
        
        isCrouching = false;
        if (_crouchCoroutine != null) StopCoroutine(_crouchCoroutine);
        _crouchCoroutine = StartCoroutine(ChangeCrouchStateCoroutine(Preset.NormalHeight, Preset.NormalCameraHeight));
        return true;
    }
    
    private bool CheckCanEndCrouch()
    {
        float radius = _characterController.radius;
        Vector3 bottom = transform.position + Vector3.up * radius;
        Vector3 top = bottom + Vector3.up * (Preset.NormalHeight - radius * 2f);
        return !Physics.CheckCapsule(bottom, top, radius, Preset.WhatIsSurface, QueryTriggerInteraction.Ignore);
        //return !Physics.SphereCast(transform.position, radius, transform.up, out RaycastHit hitInfo, Preset.NormalHeight - radius, Preset.WhatIsSurface);
    }
    
    private void RequestJump()
    {
        if (!GetCanControllable) return;
        
        _currentJumpBufferTimer = Preset.JumpBufferTime;
    }

    private void TryJump()
    {
        _currentCoyoteTimer -= Time.deltaTime;
        _currentJumpBufferTimer -= Time.deltaTime;
        
        if (!GetCanControllable) return;
        
        if (_currentJumpBufferTimer > 0 && _currentCoyoteTimer > 0)
        {
            _currentGravityVelocity.y = Mathf.Sqrt(Preset.JumpHeight * Physics.gravity.y * -2f * Preset.FallGravityMultiplier);
            _currentJumpBufferTimer = 0f;
            _currentCoyoteTimer = 0f;
            OnJump?.Invoke();
        }
    }
    #endregion
  
    
    #region Move Methods
    private void EnableLocomotion()
    {
        _canControllableCount--;
        if (_canControllableCount < 0) _canControllableCount = 0;
        if (_canControllableCount != 0) return;
        
        canControllable = true;
    }

    private void DisableLocomotion()
    {
        _canControllableCount++;
        if (_canControllableCount > 1) return;
        
        canControllable = false;
        _moveInput = Vector2.zero;
    }
    
    public void Move(float speed)
    {
        currentSpeed = speed;
        //Gravedad
        if (!isGrounded) _currentGravityVelocity.y += Physics.gravity.y * Time.deltaTime * Preset.FallGravityMultiplier;
        else
        {
            if (_currentGravityVelocity.y < 0) _currentGravityVelocity.y = AntiBumpFactor;
            _currentCoyoteTimer = Preset.CoyoteTime;
        }
        
        //Movimiento horizontal
        _moveInput = PlayerControls.Instance.InputMovement();
        _targetVelocity = transform.forward * _moveInput.y + transform.right * _moveInput.x;
        _targetVelocity.Normalize();
        _targetVelocity *= speed;
        
        //Aceleracion
        _currentMoveVelocity = Vector3.Lerp(_currentMoveVelocity, _targetVelocity, Preset.Acceleration * Time.deltaTime);
        //_currentMoveVelocity = Vector3.MoveTowards(_currentMoveVelocity, _targetVelocity, acceleration * Time.deltaTime);

        //Aplicar gravedad
        Vector3 totalVelocity = _currentMoveVelocity;
        totalVelocity.y = _currentGravityVelocity.y;
        
        //Movimiento + detección de suelo
        CollisionFlags flags = _characterController.Move(totalVelocity * Time.deltaTime);
        isGrounded = (flags & CollisionFlags.Below) != 0;
        
        if (isGrounded) OnMove?.Invoke();
        
        //Cancelar salto si chocás el techo
        if ((flags & CollisionFlags.Above) != 0 && _currentGravityVelocity.y > 0) _currentGravityVelocity.y = AntiBumpFactor;
    }

    private IEnumerator ChangeCrouchStateCoroutine(float targetHeight, float cameraHeight)
    {
        float timer = 0;
        float startHeight = _characterController.height;
        Vector3 camStartPos = _playerCameraTransform.localPosition;
        Vector3 camTargetPos = new Vector3(camStartPos.x, cameraHeight, camStartPos.z);

        while (timer < 1)
        {
            timer += Time.deltaTime / Preset.CrouchTransitionTime;
            _characterController.height = Mathf.SmoothStep(startHeight, targetHeight, timer);
            _characterController.center = new Vector3(_characterController.center.x, _characterController.height / 2, _characterController.center.z);
            _playerCameraTransform.localPosition = Vector3.Lerp(camStartPos, camTargetPos, timer);
            yield return null;
        }
        
        _characterController.height = targetHeight;
        _characterController.center = new Vector3(0, targetHeight / 2, 0);
        _playerCameraTransform.localPosition = camTargetPos;
    }
    #endregion
}