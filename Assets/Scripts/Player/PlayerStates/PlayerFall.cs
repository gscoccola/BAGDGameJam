using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerFall : MonoBehaviour, IState<PlayerState>
{
    private PlayerController _playerController;
    private FiniteStateMachine<PlayerState> _fsm;

    [Header("Fall")]
    [SerializeField, ReadOnly] private float currentFallTime;
    [SerializeField, ReadOnly] private float currentSpeed;
    
    
    public IState<PlayerState> Initialize(FiniteStateMachine<PlayerState> fsm)
    {
        _playerController = GetComponent<PlayerController>();
        _fsm = fsm;
        return this;
    }

    public void OnEnter(params object[] parameters)
    {
        _playerController.CurrentPlayerState = PlayerState.Falling;
        currentSpeed = (float) parameters[0];
        if (currentSpeed < _playerController.Preset.WalkSpeed) currentSpeed = _playerController.Preset.WalkSpeed;
        currentSpeed *= _playerController.Preset.FallingMoveSpeedMultiplier;
    }

    public void OnUpdate()
    {
        currentFallTime += Time.deltaTime;
        if (_playerController.CurrentGravityVelocity.y > 0) currentFallTime = 0;
        
        // 1. Si toco piso
        if (_playerController.GetIsGrounded)
        {
            _fsm.ChangeState(PlayerState.Landed, currentFallTime);
        }
        
        // 2. Si me puedo mover, me muevo
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
        currentSpeed = 0;
        currentFallTime = 0;
    }
}