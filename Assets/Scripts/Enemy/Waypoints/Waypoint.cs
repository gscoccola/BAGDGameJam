using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private Enemy _enemy;
    public WaypointCenter closestCenter = null;
    public WaypointCenter secondClosestCenter = null;

    private void Awake()
    {
        _enemy = FindAnyObjectByType<Enemy>();

        //WaypointCenter closestCenter = null;
        //WaypointCenter secondClosestCenter = null;
        float minDistance = Mathf.Infinity;
        float secondMinDistance = Mathf.Infinity;

        foreach (WaypointCenter center in _enemy._wayPointCenters)
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
