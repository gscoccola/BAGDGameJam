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

    private NavMeshAgent _naveMeshAgent;
    private bool _canSeePlayer;
    private Vector3 _lastKnownPlayerPos;
    

    private void Start()
    {
        _naveMeshAgent = GetComponent<NavMeshAgent>();
        if (_playerTransform == null) Debug.LogError("No Player Found");
        //CurrentState = EnemyState.Idle;

        CurrentState = EnemyState.MovingToRandomPosition;
        SwitchTowardsRandomPos(_randomPatrolDistance, _playerTransform.position);
    }

    private void Update()
    {
        if (HasLineOfSight(transform.position, _playerTransform.position))
        {
            CurrentState = EnemyState.MovingToPlayer;
            SetTarget(_playerTransform.position);
        }

        else if (FlatDistance(_playerTransform) > _maxDistanceToPlayer)
        {
            Teleport();
            CurrentState = EnemyState.MovingToRandomPosition;
            SwitchTowardsRandomPos(_randomPatrolDistance, _playerTransform.position);
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
        Physics.Raycast(origin, direction, out hit);
        return hit.point == target;
    }

    private void SetTarget(Vector3 position)
    {
        _target.position = position;
        _naveMeshAgent.SetDestination(position);
    }

    private Vector3 GetRandomPos()
    {
        Vector2 randomCircle = Random.insideUnitCircle;
        return new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    private void SwitchTowardsRandomPos(float distance, Vector3 playerPos)
    {
        NavMesh.SamplePosition(playerPos + GetRandomPos() * distance, out NavMeshHit hit, 10f, NavMesh.AllAreas);
        SetTarget(hit.position);
    }

    private float FlatDistance(Transform target)
    {
        return (transform.position - target.position).Flatten().magnitude;
    }

    private void Teleport()
    {
        int i = 0;
        while (i < 3)
        {
            if (NavMesh.SamplePosition(_playerTransform.position + GetRandomPos() *
            _teleportDistanceToPlayer, out NavMeshHit hit, 1f, NavMesh.AllAreas) &&
            !HasLineOfSight(transform.position, hit.position))
            {
                transform.position = hit.position;
                SwitchTowardsRandomPos(_randomPatrolDistance, _playerTransform.position);
                CurrentState = EnemyState.MovingToRandomPosition;
                break;
            }

            i++;
        } 
    }
}


public enum EnemyState
{
    Idle,
    MovingToLastKnownPosition,
    MovingToPlayer,
    MovingToRandomPosition
}

public static class VectorExtensions
{
    public static Vector3 Flatten(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.z);
    }
}
