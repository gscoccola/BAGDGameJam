using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerSprint : MonoBehaviour, IState<PlayerState>
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