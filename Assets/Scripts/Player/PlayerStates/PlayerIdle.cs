using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerIdle : MonoBehaviour, IState<PlayerState>
{
    private PlayerController _playerController;
    private FiniteStateMachine<PlayerState> _fsm;
    

    public IState<PlayerState> Initialize(FiniteStateMachine<PlayerState> fsm)
    {
        _playerController = GetComponent<PlayerController>();
        _fsm = fsm;
        return this;
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