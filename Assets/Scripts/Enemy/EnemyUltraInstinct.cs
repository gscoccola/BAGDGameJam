using UnityEngine;

public class EnemyUltraInstinct : IState<EnemyState>
{
    private FiniteStateMachine<EnemyState> _stateMachine;
    private Enemy _enemy;
    private float _timer;

    public EnemyUltraInstinct(params object[] parameters)
    {
        _enemy = (Enemy)parameters[0];
        _stateMachine = (FiniteStateMachine<EnemyState>)parameters[1];
    }

    public void OnEnter(params object[] parameters)
    {
        _timer = 0f;
        _enemy.CurrentState = EnemyState.UltraInstinct;
    }

    public void OnExit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
        _timer += Time.deltaTime;
        if (_timer >= _enemy._ultraInstinctTime)
        {
            _stateMachine.ChangeState(EnemyState.Search, true);
            return;
        }
        _enemy.SetTargetAndNavMeshPath(_enemy._playerTransform.position);
    }
}
