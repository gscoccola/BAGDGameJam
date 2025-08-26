public class PlayerCrouch : IState<PlayerState>
{
    private readonly PlayerController _playerController;
    private readonly FiniteStateMachine<PlayerState> _fsm;

    
    public PlayerCrouch (params object[] parameters)
    {
        _fsm = (FiniteStateMachine<PlayerState>) parameters[0];
        _playerController = (PlayerController) parameters[1];
    }
    
    public void OnEnter(params object[] parameters)
    {
        _playerController.CurrentPlayerState = PlayerState.Crouching;
    }
    
    public void OnUpdate()
    {
        // 1. Prioridad maxima si estas en el aire
        if (!_playerController.GetIsGrounded)
        {
            _fsm.ChangeState(PlayerState.Falling, _playerController.Preset.CrouchSpeed);
            return;
        }
        // 2. No se puede controlar o no hay input
        if (!_playerController.GetCanControllable || PlayerControls.Instance.InputMovement().magnitude == 0)
        {
            _fsm.ChangeState(PlayerState.Idle);
            return;
        }
        
        // 3. Si deja de estar agachado, va caminando
        if (!_playerController.GetIsCrouching)
        {
            _fsm.ChangeState(PlayerState.Walking);
            return;
        }
        
        // 4. Si está sprintando mientras agachado
        if (_playerController.GetIsSprinting)
        {
            bool canSprint = _playerController.Preset.CanSprintBackward || !_playerController.IsMovingBackward;
            if (canSprint)
            {
                if (!_playerController.StopCrouch()) _playerController.Move(_playerController.Preset.CrouchSpeed);
            }
            else // 5. Me muevo agachado
            {
                _playerController.Move(_playerController.Preset.CrouchSpeed);
            }
            return;
        }
        
        // 5. Me muevo agachado
        _playerController.Move(_playerController.Preset.CrouchSpeed);
    }

    public void OnExit()
    {
        
    }
}