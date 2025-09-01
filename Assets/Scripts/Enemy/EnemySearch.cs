using UnityEngine;

public class EnemySearch : IState<EnemyState>
{
    private FiniteStateMachine<EnemyState> _stateMachine;
    private Enemy _enemy;
    private float _timer;
    private Vector3 _lastKnownPlayerPosition;
    private bool _hasChecked;

    public EnemySearch(params object[] parameters)
    {
        _enemy = (Enemy)parameters[0];
        _stateMachine = (FiniteStateMachine<EnemyState>)parameters[1];
    }

    public void OnEnter(params object[] parameters)
    {
        bool value = (bool)parameters[0];
        _timer = value ? _enemy._maxSearchTime : 0f;
        _hasChecked = false;
        _lastKnownPlayerPosition = Vector3.zero;
        _enemy.CurrentState = EnemyState.Search;
    }

    public void OnExit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
        _timer += Time.deltaTime;
        if (_enemy.HasLineOfSight(_enemy.transform.position))
        {
            _stateMachine.ChangeState(EnemyState.Chase);
            return;
        }
        
        if (_timer >= _enemy._maxSearchTime)
        {
            if (!_hasChecked)
            {
                _lastKnownPlayerPosition = _enemy._playerTransform.position;
                _enemy.SetTargetAndNavMeshPath(_enemy._playerTransform.position);
                _hasChecked = true;
                if (_timer >= _enemy._maxSearchTime * 2) _stateMachine.ChangeState(EnemyState.Patrol);
            }
            if (Vector3.Distance(_lastKnownPlayerPosition, _enemy.transform.position) < 5f)
            {
                _stateMachine.ChangeState(EnemyState.Patrol);
                return;

            }
        }
        else _enemy.SetTargetAndNavMeshPath(_enemy._playerTransform.position);
    }
}
