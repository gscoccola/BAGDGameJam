using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerWalk : MonoBehaviour, IState<PlayerState>
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
        _playerController.CurrentPlayerState = PlayerState.Walking;
    }

    public void OnUpdate()
    {
        // 1. Prioridad maxima si esta en el aire
        if (!_playerController.GetIsGrounded)
        {
            _fsm.ChangeState(PlayerState.Falling, _playerController.Preset.WalkSpeed);
            return;
        }
        
        // 2. No se puede controlar o no hay input
        if (!_playerController.GetCanControllable || PlayerControls.Instance.InputMovement().magnitude == 0)
        {
            _fsm.ChangeState(PlayerState.Idle);
            return;
        }
        
        // 3. Sprinting (si se permite)
        if (_playerController.GetIsSprinting)
        {
            bool canSprint = _playerController.Preset.CanSprintBackward || !_playerController.IsMovingBackward;
            if (canSprint)
            {
                _fsm.ChangeState(PlayerState.Sprinting);
            }
            else // Camino
            {
                _playerController.Move(_playerController.Preset.WalkSpeed);
            }
            return;
        }
        
        // 4. Crouching
        if (_playerController.GetIsCrouching)
        {
            _fsm.ChangeState(PlayerState.Crouching);
            return;
        }
        
        // 5. Camino
        _playerController.Move(_playerController.Preset.WalkSpeed);
    }

    public void OnExit()
    {
        
    }
}