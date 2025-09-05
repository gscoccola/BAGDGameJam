using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public GameObject haveASheep;
    
    [Header("Controller")]
    [SerializeField] private PlayerPreset preset;
    public PlayerPreset Preset => preset;
    private Transform _playerCameraTransform;
    private Coroutine _crouchCoroutine;
    
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
        _characterController.slopeLimit = preset.MaxSlideAngle - 0.1f;
        _playerCameraTransform = GetComponentInChildren<PlayerCameraController>().transform;
        _characterController.height = Preset.NormalHeight;
        _characterController.center = new Vector3(0, Preset.NormalHeight / 2, 0);
        Vector3 camStartPos = _playerCameraTransform.localPosition;
        Vector3 camTargetPos = new Vector3(camStartPos.x, Preset.NormalCameraHeight, camStartPos.z);
        _playerCameraTransform.localPosition = camTargetPos;
        PlayerSounds = GetComponentInChildren<PlayerSounds>();
        isSprinting = true;
        
        EnableLocomotion();
        PlayerControls.Instance.OnSprintKeyStarted += StartPressSprint;
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
        
        EventManager.Instance.Subscribe(Constants.OnPlayerKeepSheepForSounds, EnableSHeep);
    }

    private void EnableSHeep(params object[] parameters)
    {
        if (parameters != null && parameters.Length > 0)
        {
            var asd = (GameObject)parameters[0];
            if (asd != null)
            {
                haveASheep = asd;
            }
            else
            {
                haveASheep = null;
            }
        }
        else
        {
            haveASheep = null;
        }
    }
    
    
    private void OnDestroy()
    {
        //PlayerControls.Instance.OnSprintKey -= StartPressSprint;
        //PlayerControls.Instance.OnSprintKeyCancelled -= StopPressSprint;
        //PlayerControls.Instance.OnCrouchKey -= PressCrouch;
        //PlayerControls.Instance.OnJumpKey -= RequestJump;
        //EventManager.Instance.Unsubscribe(Constants.OnPlayerDeath, DisableLocomotion);
        //EventManager.Instance.Unsubscribe(Constants.OnEnableControls, EnableLocomotion);
        //EventManager.Instance.Unsubscribe(Constants.OnDisableControls, DisableLocomotion);
    }

    private void Update()
    {
        Fsm.Update();
    }
    
    public void ActiveLandEvent() { OnLand?.Invoke(); }

    
    #region Inputs Methods
    private void StartPressSprint()
    {
        isSprinting = !isSprinting;
    }
    
    private void PressCrouch()
    {
        if (!GetCanControllable || !GetIsGrounded) return;
        
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

        if (isGrounded)
        {
            _currentGravityVelocity.y = Mathf.Sqrt(Preset.JumpHeight * Physics.gravity.y * -2f * Preset.FallGravityMultiplier);
            isGrounded = false;
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
        
        //Movimiento
        CollisionFlags flags = _characterController.Move(totalVelocity * Time.deltaTime);
        
        //Detección de suelo
        if ((flags & CollisionFlags.Below) != 0)
        {
            IsSlidingOnSlope();
        }
        else
        {
            isGrounded = false;
        }

        
        if (isGrounded) OnMove?.Invoke();
        
        //Cancelar salto si chocás el techo
        if ((flags & CollisionFlags.Above) != 0 && _currentGravityVelocity.y > 0) _currentGravityVelocity.y = AntiBumpFactor;
    }

    private void IsSlidingOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _characterController.height / 2 + 0.2f, preset.WhatIsSurface, QueryTriggerInteraction.Ignore))
        {
            //Comprobar si la pendiente es demasiado empinada
            float slotAngle = Vector3.Angle(hit.normal, Vector3.up);

            if (slotAngle <= preset.MaxSlideAngle)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
                Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                _characterController.Move(slideDirection * (Mathf.Abs(_currentGravityVelocity.y) * Time.deltaTime));
            }
        }
        else
        {
            isGrounded = true;
        }
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