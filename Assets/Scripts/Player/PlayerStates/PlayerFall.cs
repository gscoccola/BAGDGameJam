using UnityEngine;

public class PlayerFall : IState<PlayerState>
{
    private readonly PlayerController _playerController;
    private readonly FiniteStateMachine<PlayerState> _fsm;
    
    private float currentFallTime;
    private float currentSpeed;
    
    
    public PlayerFall (params object[] parameters)
    {
        _fsm = (FiniteStateMachine<PlayerState>) parameters[0];
        _playerController = (PlayerController) parameters[1];
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
            if (_playerController.GetIsSprinting && currentSpeed >= _playerController.Preset.minSprintSpeed)
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