public class PlayerIdle : IState<PlayerState>
{
    private readonly PlayerController _playerController;
    private readonly FiniteStateMachine<PlayerState> _fsm;
    

    public PlayerIdle (params object[] parameters)
    {
        _fsm = (FiniteStateMachine<PlayerState>) parameters[0];
        _playerController = (PlayerController) parameters[1];
    }

    public void OnEnter(params object[] parameters)
    {
        _playerController.CurrentPlayerState = PlayerState.Idle;
    }

    public void OnUpdate()
    {
        // 1. Prioridad maxima si esta en el aire
        if (!_playerController.GetIsGrounded)
        {
            _fsm.ChangeState(PlayerState.Falling, 0f);
            return;
        }
        
        // 2. Si se puede controlar y hay input
        if (_playerController.GetCanControllable && PlayerControls.Instance.InputMovement().magnitude > 0)
        {
            if (_playerController.GetIsCrouching) // Me agacho
            {
                _fsm.ChangeState(PlayerState.Crouching);
            }
            else // Camino
            {
                _fsm.ChangeState(PlayerState.Walking);
            }
            return;
        }
        
        // 3. Me quedo quieto
        _playerController.Move(0);
    }

    public void OnExit()
    {
        
    }
}