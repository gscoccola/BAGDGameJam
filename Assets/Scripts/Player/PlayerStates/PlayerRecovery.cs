using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerRecovery : MonoBehaviour, IState<PlayerState>
{
    private PlayerController _playerController;
    private FiniteStateMachine<PlayerState> _fsm;
    
    [Header("Recovery")]
    [SerializeField, ReadOnly] private float currentDuration;
    
    
    public IState<PlayerState> Initialize(FiniteStateMachine<PlayerState> fsm)
    {
        _playerController = GetComponent<PlayerController>();
        _fsm = fsm;
        return this;
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
        else if (_playerController.GetIsSprinting) currentSpeed = _playerController.Preset.SprintSpeed;
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
            if (_playerController.GetIsSprinting && currentSpeed >= _playerController.Preset.SprintSpeed)
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