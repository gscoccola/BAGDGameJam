using UnityEngine;


public class PlayerRecovery :  IState<PlayerState>
{
    private PlayerController _playerController;
    private FiniteStateMachine<PlayerState> _fsm;
    
    [Header("Recovery")]
    [SerializeField, ReadOnly] private float currentDuration;
    
    
    public PlayerRecovery (params object[] parameters)
    {
        _fsm = (FiniteStateMachine<PlayerState>) parameters[0];
        _playerController = (PlayerController) parameters[1];
    }

    public void OnEnter(params object[] parameters)
    {
        _playerController.CurrentPlayerState = PlayerState.Recovery;
    }

    public void OnUpdate()
    {
        currentDuration += Time.deltaTime;
        float currentSpeed;
        if (PlayerControls.Instance.InputMovement().magnitude == 0) currentSpeed = 0f;
        else if (_playerController.GetIsSprinting) currentSpeed = _playerController.Preset.sprintSpeed;
        else if (_playerController.GetIsCrouching) currentSpeed = _playerController.Preset.CrouchSpeed;
        else currentSpeed = _playerController.Preset.WalkSpeed;
        
        // 1. Prioridad maxima si esta en el aire
        if (!_playerController.GetIsGrounded) 
        {
            _fsm.ChangeState(PlayerState.Falling, currentSpeed);
            return;
        }
        
        // 2. Si termina la duracion del estado
        if (currentDuration >= _playerController.Preset.RecoveryDuration)
        {
            _fsm.ChangeState(PlayerState.Idle);
            return;
        }
        
        // 3. Si me puedo mover, me muevo
        if (_playerController.GetCanControllable)
        {
            // Si estoy corriendo de espaldas y no puedo
            if (_playerController.GetIsSprinting && currentSpeed >= _playerController.Preset.sprintSpeed)
            {
                bool canSprint = _playerController.Preset.CanSprintBackward || !_playerController.IsMovingBackward;
                if (!canSprint) currentSpeed = _playerController.Preset.WalkSpeed;
            }
            _playerController.Move(currentSpeed);
        }
    }

    public void OnExit()
    {
        currentDuration = 0;
    }
}