using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private FiniteStateMachine<EnemyState> _stateMachine;
    private NavMeshAgent _naveMeshAgent;
    private bool _canSeePlayer;
    private Vector3 _lastKnownPlayerPos;
    private bool _playerInSafeZone;

    [Header("Parameters")]
    [SerializeField] public float _randomPatrolDistance;
    [SerializeField] public float _maxDistanceToPlayer;
    [SerializeField] public float _teleportDistanceToPlayer;
    [SerializeField] public float _maxSearchTime;
    [SerializeField] public float _ultraInstinctTime;

    [Header("References")]
    [SerializeField] public Transform _target;
    [SerializeField] public Transform _playerTransform;

    [Header("Debug")]
    [ReadOnly] public EnemyState CurrentState;
    [ReadOnly] public float DistanceToTarget;
    [ReadOnly] public float DistanceToPlayer;

    private void Awake()
    {
        _stateMachine = new FiniteStateMachine<EnemyState>();
        _stateMachine.AddState(EnemyState.Patrol, new EnemyPatrol(this, _stateMachine));
        _stateMachine.AddState(EnemyState.Chase, new EnemyChase(this, _stateMachine));
        _stateMachine.AddState(EnemyState.Search, new EnemySearch(this, _stateMachine));
        _stateMachine.AddState(EnemyState.UltraInstinct, new EnemyUltraInstinct(this, _stateMachine));
        EventManager.Instance.Subscribe(Constants.OnSheepPickup, OnSheepPickup);
        EventManager.Instance.Subscribe(Constants.OnSheepNoise, OnSheepNoise);
        _stateMachine.ChangeState(EnemyState.Patrol);
        _naveMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        DistanceToTarget = FlatDistance(_target);
        DistanceToPlayer = FlatDistance(_playerTransform);
        _stateMachine.Update();
    }

    public bool HasLineOfSight(Vector3 origin)
    {
        RaycastHit hit;
        Vector3 direction = (_playerTransform.position - origin).normalized;
        if (!Physics.Raycast(origin, direction, out hit)) return false;
        return hit.transform.position == _playerTransform.position;
    }

    public void SetTargetAndNavMeshPath(Vector3 position)
    {
        _target.position = position;
        _naveMeshAgent.SetDestination(position);
    }

    public float FlatDistance(Transform target)
    {
        return (transform.position - target.position).Flatten().magnitude;
    }

    /*public void Teleport()
    {
        Vector3 tryPos = _playerTransform.position.Flatten() + GetRandomPos() * _teleportDistanceToPlayer;
        if (NavMesh.SamplePosition(tryPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            if (HasLineOfSight(hit.position, _playerTransform.position))
            {
                //Debug.Log("Teleport was in line of sight of player");
                return;
            }
            transform.position = hit.position;
            SwitchTowardsRandomPos(_randomPatrolDistance, _playerTransform.position);
            CurrentState = EnemyState.MovingToRandomPosition;
            //Debug.Log(FlatDistance(_playerTransform));
        }
        //else Debug.Log("Random Pos was too far from mesh");
    }*/

    /*public void EnterSafeZone()
    {
        _playerInSafeZone = true;
        CurrentState = EnemyState.Idle;
        _naveMeshAgent.isStopped = true;
    
    }*/

    /*public void ExitSafeZone()
    {
        _playerInSafeZone = false;
        _naveMeshAgent.isStopped = false;
        CurrentState = EnemyState.MovingToLastKnownPosition;
        SetTargetAndNavMeshPath(_playerTransform.position);
    }*/

    public void OnSheepPickup()
    {
        if (CurrentState == EnemyState.UltraInstinct) return;
        _stateMachine.ChangeState(EnemyState.UltraInstinct);
    }

    public void OnSheepNoise()
    {
        if (CurrentState == EnemyState.UltraInstinct ||
            CurrentState == EnemyState.Search) return;
        _stateMachine.ChangeState(EnemyState.Search, false);
    }
}
