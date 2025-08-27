using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _randomPatrolDistance;
    [SerializeField] private float _maxDistanceToPlayer;
    [SerializeField] private float _teleportDistanceToPlayer;

    [Header("References")]
    [SerializeField] private NavMeshSurface _navMesh;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _playerTransform;

    [Header("Debug")]
    public EnemyState CurrentState;
    public float DistanceToTarget;
    public float DistanceToPlayer;

    private NavMeshAgent _naveMeshAgent;
    private bool _canSeePlayer;
    private Vector3 _lastKnownPlayerPos;
    private bool _playerInSafeZone;

    private void Start()
    {
        EventManager.Instance.Subscribe("EnterSafeZone", EnterSafeZone);
        EventManager.Instance.Subscribe("ExitSafeZone", ExitSafeZone);

        _naveMeshAgent = GetComponent<NavMeshAgent>();
        if (_playerTransform == null) Debug.LogError("No Player Found");
        //CurrentState = EnemyState.Idle;

        CurrentState = EnemyState.MovingToLastKnownPosition;
        SetTarget(_playerTransform.position);
    }

    private void Update()
    {
        DistanceToTarget = FlatDistance(_target);
        DistanceToPlayer = FlatDistance(_playerTransform);

        if (FlatDistance(_playerTransform) < 0.1f)
        {
            CurrentState = EnemyState.FoundPlayer;
            SetTarget(transform.position);
        }

        else if (HasLineOfSight(transform.position, _playerTransform.position))
        {
            CurrentState = EnemyState.MovingToPlayer;
            SetTarget(_playerTransform.position);
        }

        else if (FlatDistance(_playerTransform) > _maxDistanceToPlayer)
        {
            Teleport();
            
        }

        else if (CurrentState == EnemyState.MovingToPlayer)
        {
            CurrentState = EnemyState.MovingToLastKnownPosition;
        }

        else if ((CurrentState == EnemyState.MovingToLastKnownPosition || CurrentState == EnemyState.MovingToRandomPosition) &&
             FlatDistance(_target) < 1f)
        {
            CurrentState = EnemyState.MovingToRandomPosition;
            SwitchTowardsRandomPos(_randomPatrolDistance, _playerTransform.position);
        }
    }

    private bool HasLineOfSight(Vector3 origin, Vector3 target)
    {
        RaycastHit hit;
        Vector3 direction = (target - origin).normalized;
        if (!Physics.Raycast(origin, direction, out hit)) return false;
        return hit.transform.position == target;
    }

    private void SetTarget(Vector3 position)
    {
        _target.position = position;
        _naveMeshAgent.SetDestination(position);
    }

    private Vector3 GetRandomPos()
    {
        Vector2 randomCircle = Random.insideUnitCircle;
        randomCircle = randomCircle.normalized;
        return new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    private void SwitchTowardsRandomPos(float distance, Vector3 playerPos)
    {
        if (NavMesh.SamplePosition(playerPos.Flatten() + GetRandomPos() * distance, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            SetTarget(hit.position);
        //else Debug.Log("Random Pos was too far from mesh");
    }

    private float FlatDistance(Transform target)
    {
        return (transform.position - target.position).Flatten().magnitude;
    }

    private void Teleport()
    {
        Vector3 tryPos = _playerTransform.position.Flatten() + GetRandomPos() * _teleportDistanceToPlayer;
        /*Debug.Log("PlayerPos " + _playerTransform.position.Flatten());
        Debug.Log("Trying to teleport to " + tryPos);*/
        if (NavMesh.SamplePosition(tryPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            if (HasLineOfSight(hit.position , _playerTransform.position))
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
    }

    private void EnterSafeZone()
    {
        _playerInSafeZone = true;
        CurrentState = EnemyState.Idle;
        _naveMeshAgent.isStopped = true;

    }

    private void ExitSafeZone()
    {
        _playerInSafeZone = false;
        _naveMeshAgent.isStopped = false;
        CurrentState = EnemyState.MovingToLastKnownPosition;
        SetTarget(_playerTransform.position);
    }
}


public enum EnemyState
{
    Idle,
    MovingToLastKnownPosition,
    MovingToPlayer,
    MovingToRandomPosition,
    FoundPlayer,
    Patrol,
    Chase,
    Search,
    UltraInstinct,
}

public static class VectorExtensions
{
    public static Vector3 Flatten(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }
}
