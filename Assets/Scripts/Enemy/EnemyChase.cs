using UnityEngine;

public class EnemyChase : IState<EnemyState>
{

    private FiniteStateMachine<EnemyState> _stateMachine;
    private Enemy _enemy;

    public EnemyChase(params object[] parameters)
    {
        _enemy = (Enemy)parameters[0];
        _stateMachine = (FiniteStateMachine<EnemyState>)parameters[1];
    }


    public void OnEnter(params object[] parameters)
    {
        _enemy.CurrentState = EnemyState.Chase;
    }

    public void OnExit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
        if (!_enemy.HasLineOfSight(_enemy.transform.position))
        {
            _stateMachine.ChangeState(EnemyState.Search, false);
            return;
        }
        _enemy.SetTargetAndNavMeshPath(_enemy._playerTransform.position);
    }
}
