using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private FiniteStateMachine<EnemyState> _stateMachine;
    private NavMeshAgent _naveMeshAgent;
    public bool _playerInSafeZone;
    //public List<Vector3> _wayPointlist = new();
    private PlayerHealth _playerHealth;

    [Header("Parameters")]
    /*[SerializeField] public float _randomPatrolDistance;
    [SerializeField] public float _maxDistanceToPlayer;
    [SerializeField] public float _teleportDistanceToPlayer;*/
    [SerializeField] public float _maxSearchTime;
    [SerializeField] public float _ultraInstinctTime;
    [SerializeField] public float _damageDistance;
    [SerializeField] public float _damagePerSecond;

    
    

    [Header("References")]
    [SerializeField] public Transform _target;
    [SerializeField] public Transform _playerTransform;
    //[SerializeField] public Transform[] _wayPoints;
    [SerializeField] public WaypointCenter[] _wayPointCenters;


    [Header("Debug")]
    [ReadOnly] public EnemyState CurrentState;
    [ReadOnly] public float DistanceToTarget;
    [ReadOnly] public float DistanceToPlayer;
    [ReadOnly] public float ClosestCenterToPlayer;

    private void Awake()
    {
        _stateMachine = new FiniteStateMachine<EnemyState>();
        _stateMachine.AddState(EnemyState.Patrol, new EnemyPatrol(this, _stateMachine));
        _stateMachine.AddState(EnemyState.Chase, new EnemyChase(this, _stateMachine));
        _stateMachine.AddState(EnemyState.Search, new EnemySearch(this, _stateMachine));
        _stateMachine.AddState(EnemyState.UltraInstinct, new EnemyUltraInstinct(this, _stateMachine));
        EventManager.Instance.Subscribe(Constants.OnSheepPickup, OnSheepPickup);
        EventManager.Instance.Subscribe(Constants.OnSheepNoise, OnSheepNoise);
        EventManager.Instance.Subscribe(Constants.OnEnterSafeZone, EnterSafeZone);
        EventManager.Instance.Subscribe(Constants.OnExitSafeZone, ExitSafeZone);
        _naveMeshAgent = GetComponent<NavMeshAgent>();
        _playerHealth = _playerTransform.gameObject.GetComponent<PlayerHealth>();
        /*foreach (Transform wp in _wayPoints)
        {
            _wayPointlist.Add(wp.position);
        }*/
        
    }

    private void Start()
    {
        _stateMachine.ChangeState(EnemyState.Patrol);
    }

    private void Update()
    {
        DistanceToTarget = FlatDistance(_target);
        DistanceToPlayer = FlatDistance(_playerTransform);
        if (DistanceToPlayer < _damageDistance)
        {
            _playerHealth.TakeDamage(_damagePerSecond * Time.deltaTime);
        }

        _stateMachine.Update();
    }

    public bool HasLineOfSight(Vector3 origin)
    {
        RaycastHit hit;
        Vector3 direction = (_playerTransform.position - origin).normalized;
        if (!Physics.Raycast(origin, direction, out hit)) return false;
        Debug.DrawRay(origin, direction * hit.distance, Color.red);
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
    public float FlatDistance(Vector3 target)
    {
        return (transform.position - target).Flatten().magnitude;
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

    public void EnterSafeZone()
    {
        _playerInSafeZone = true;
        _stateMachine.ChangeState(EnemyState.Patrol);

    }

    public void ExitSafeZone()
    {
        _playerInSafeZone = false;
    }

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

    public WaypointCenter GetClosestCenter()
    {
        var result = _wayPointCenters[0];
        float minDistance = Vector3.Distance( result.transform.position, _playerTransform.position);
        for (int i = 1; i < _wayPointCenters.Length; i++)
        {
            float distance = Vector3.Distance(_wayPointCenters[i].transform.position, _playerTransform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                result = _wayPointCenters[i];
            }
        }
        return result;
    }
}


public enum EnemyState
{
    Idle,
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

public static class ListExtension
{
    public static List<T> Shuffle<T>(this List<T> input)
    {
        List<T> list = new List<T>(input);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
}
