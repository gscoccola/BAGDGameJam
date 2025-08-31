using UnityEngine;
using UnityEngine.AI;

public class Waypoint : MonoBehaviour
{
    private Enemy _enemy;
    public WaypointCenter closestCenter = null;
    public WaypointCenter secondClosestCenter = null;

    private void Start()
    {
        _enemy = FindAnyObjectByType<Enemy>();


        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 50f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        else
        {
            Debug.LogError("Waypoint could not find a valid NavMesh position.");
        }

        float minDistance = Mathf.Infinity;
        float secondMinDistance = Mathf.Infinity;

        foreach (WaypointCenter center in _enemy._centers)
        {
            float distance = Vector3.Distance(transform.position, center.transform.position);
            if (distance < minDistance)
            {
                secondMinDistance = minDistance;
                secondClosestCenter = closestCenter;

                minDistance = distance;
                closestCenter = center;
            }
            else if (distance < secondMinDistance)
            {
                secondMinDistance = distance;
                secondClosestCenter = center;
            }
        }

        closestCenter.ConnectedWaypointPositions.Add(transform.position);
        secondClosestCenter.ConnectedWaypointPositions.Add(transform.position);
        // Destroy(gameObject, 0.1f);
    }
}
