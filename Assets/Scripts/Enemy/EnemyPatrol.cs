using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class EnemyPatrol : IState<EnemyState>
{
    private FiniteStateMachine<EnemyState> _stateMachine;
    private Enemy _enemy;
    private List<Vector3> _wayPointsQueue = new List<Vector3>();
    private Vector3 _currentWayPoint;

    public EnemyPatrol(params object[] parameters)
    {
        _enemy = (Enemy)parameters[0];
        _stateMachine = (FiniteStateMachine<EnemyState>)parameters[1];
    }

    public void OnEnter(params object[] parameters)
    {
        _enemy.CurrentState = EnemyState.Patrol;
        SetNewWayPoint();
    }

    public void OnExit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
        if (!_enemy._playerInSafeZone && _enemy.HasLineOfSight(_enemy.transform.position))
        {
            _stateMachine.ChangeState(EnemyState.Chase);
            return;
        }
        if (_enemy.FlatDistance(_currentWayPoint) < 0.5f)
        {
            SetNewWayPoint();
        }

    }

    private void SetNewWayPoint()
    {
        if (_wayPointsQueue.Count == 0)
        {
            _wayPointsQueue = GetWayPointsQueue();
            Debug.Log("new queue");
        }
        Debug.Log("next waypoint");
        _currentWayPoint = _wayPointsQueue[0];
        _wayPointsQueue.RemoveAt(0);
        _enemy.SetTargetAndNavMeshPath(_currentWayPoint);
    }

    public List<Vector3> GetWayPointsQueue()
    {
        return _enemy.GetClosestCenter().ConnectedWaypointPositions.Shuffle();
    }
}


