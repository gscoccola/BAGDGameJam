using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : IState<EnemyState>
{
    private FiniteStateMachine<EnemyState> _stateMachine;
    private Enemy _enemy;


    public EnemyPatrol(params object[] parameters)
    {
        _enemy = (Enemy)parameters[0];
        _stateMachine = (FiniteStateMachine<EnemyState>)parameters[1];
    }

    public void OnEnter(params object[] parameters)
    {
        _enemy.CurrentState = EnemyState.Patrol;
    }

    public void OnExit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
        if (_enemy.HasLineOfSight(_enemy.transform.position))
        {
            _stateMachine.ChangeState(EnemyState.Chase);
            return;
        }

   
    }

    public void SwitchTowardsRandomPos()
    {
        if (NavMesh.SamplePosition(_enemy._playerTransform.position.Flatten() + GetRandomPos() * _enemy._randomPatrolDistance, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            _enemy.SetTargetAndNavMeshPath(hit.position);
        //else Debug.Log("Random Pos was too far from mesh");
    }

    public Vector3 GetRandomPos()
    {
        Vector2 randomCircle = Random.insideUnitCircle;
        randomCircle = randomCircle.normalized;
        return new Vector3(randomCircle.x, 0, randomCircle.y);
    }
}
