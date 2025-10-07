using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint_Wanderpoint : Waypoint
{
    public List<Waypoint> WanderingSpots = new List<Waypoint>();

    protected override void OnDrawGizmosSelected()
    {
        if (Neighbors.Count <= 0) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gameObject.transform.position, 0.5f);
        foreach (var neighbor in Neighbors)
        {
            Gizmos.DrawLine(gameObject.transform.position, neighbor.transform.position);
        }
    }
}
