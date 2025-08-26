public class PlayerSprint :  IState<PlayerState>
{
    private PlayerController _playerController;
    private FiniteStateMachine<PlayerState> _fsm;
    
    
    public PlayerSprint (params object[] parameters)
    {
        _fsm = (FiniteStateMachine<PlayerState>) parameters[0];
        _playerController = (PlayerController) parameters[1];
    }
    
    public void OnEnter(params object[] parameters)
    {
        _playerController.CurrentPlayerState = PlayerState.Sprinting;
    }
    
    public void OnUpdate()
    {
        // 1. Prioridad maxima si esta en el aire
        if (!_playerController.GetIsGrounded)
        {
            _fsm.ChangeState(PlayerState.Falling, _playerController.Preset.SprintSpeed);
            return;
        }
        
        // 2. No se puede controlar o no hay input
        if (!_playerController.GetCanControllable || PlayerControls.Instance.InputMovement().magnitude == 0)
        {
            _fsm.ChangeState(PlayerState.Idle);
            return;
        }
        
        // 3. Si no está sprintando, camino
        if (!_playerController.GetIsSprinting)
        {
            _fsm.ChangeState(PlayerState.Walking);
            return;
        }
        
        // 4. Si está esprintando pero va hacia atrás y no se permite, camino
        if (!_playerController.Preset.CanSprintBackward && _playerController.IsMovingBackward)
        {
            _fsm.ChangeState(PlayerState.Walking);
            return;
        }
        
        // 5. Corro
        _playerController.Move(_playerController.Preset.SprintSpeed);
    }

    public void OnExit()
    {
        
    }
}