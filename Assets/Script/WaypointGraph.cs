using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointGraph : MonoBehaviour
{
    public static WaypointGraph Instance;
    public List<Waypoint> Waypoints;
    public List<Waypoint_Endpoint> Endpoints;
    public List<Waypoint_Wanderpoint> Wanderpoints;

    public void INIT()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        for (int i = 0; i < transform.childCount; i++)
        {
            Waypoint waypoint = transform.GetChild(i).GetComponent<Waypoint>();
            Waypoints.Add(waypoint);
            if (waypoint.GetType() == typeof(Waypoint_Endpoint))
            {
                Endpoints.Add(transform.GetChild(i).GetComponent<Waypoint_Endpoint>());
            }
            else if(waypoint.GetType() == typeof(Waypoint_Wanderpoint))
            {
                Wanderpoints.Add(transform.GetChild(i).GetComponent<Waypoint_Wanderpoint>());
            }
        }
    }

    public List<Waypoint> GetWaypoints()
    {
        return Waypoints;
    }

    private void OnDrawGizmosSelected()
    {
        // Does not need to do anything cause the waypoint script already did it lol
    }
}
