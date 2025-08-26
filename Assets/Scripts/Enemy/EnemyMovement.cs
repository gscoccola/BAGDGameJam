using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent _naveMeshAgent;
    private Transform _playerTransform;
    private bool _canSeePlayer;
    private Vector3 _lastKnownPlayerPos;

    private void Start()
    {
        _naveMeshAgent = GetComponent<NavMeshAgent>();
        _playerTransform = FindAnyObjectByType<PlayerController>().transform;
    }

    private void Update()
    {
        if (_playerTransform == null) 
        { 
            Debug.LogError("No Player Found");
            return;
        };


        if (HasLineOfSight(_playerTransform))
        {
            _lastKnownPlayerPos = _playerTransform.position;
            _naveMeshAgent.SetDestination(_lastKnownPlayerPos);
        }
    }

    private bool HasLineOfSight(Transform target)
    {
        RaycastHit hit;
        Vector3 direction = (target.position - transform.position).normalized;
        Physics.Raycast(transform.position, direction, out hit);
        return hit.transform == target;
    }
}
