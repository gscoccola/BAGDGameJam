using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _randomPatrolDistance;

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
        TowardsRandomPosition(_randomPatrolDistance, _playerTransform.position);
    }

    private void Update()
    {
        if (HasLineOfSight(_playerTransform))
        {
            CurrentState = EnemyState.MovingToPlayer;
            SetTarget(_playerTransform.position);
        }

        else if (CurrentState == EnemyState.MovingToPlayer) 
        {
            CurrentState = EnemyState.MovingToLastKnownPosition;
        }

        else if ((CurrentState == EnemyState.MovingToLastKnownPosition || CurrentState == EnemyState.MovingToRandomPosition) && 
            (transform.position - _target.position).Flatten().magnitude < 1f)
        {
            CurrentState = EnemyState.MovingToRandomPosition;
            TowardsRandomPosition(_randomPatrolDistance, _playerTransform.position);
        }
    }

    private bool HasLineOfSight(Transform target)
    {
        RaycastHit hit;
        Vector3 direction = (target.position - transform.position).normalized;
        Physics.Raycast(transform.position, direction, out hit);
        return hit.transform == target;
    }

    private void SetTarget(Vector3 position)
    {
        _target.position = position;
        _naveMeshAgent.SetDestination(position);
    }

    private Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle;
        return new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    private void TowardsRandomPosition(float distance, Vector3 playerPos)
    {
        NavMesh.SamplePosition(playerPos + GetRandomPosition() * distance, out NavMeshHit hit, 10f, NavMesh.AllAreas);
        SetTarget(hit.position);
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
