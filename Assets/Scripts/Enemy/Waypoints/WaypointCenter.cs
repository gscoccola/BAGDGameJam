using UnityEngine;
using System.Collections.Generic;

public class WaypointCenter : MonoBehaviour
{
    public List<Vector3> ConnectedWaypointPositions;

    private void Awake()
    {
        FindAnyObjectByType<Enemy>()._centers.Add(this);
    }
}
