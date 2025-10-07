using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint_Endpoint : Waypoint
{
    public WorkPlaceController controller;
    protected override void OnDrawGizmosSelected()
    {
        if (Neighbors.Count <= 0) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, 0.5f);
        foreach (var neighbor in Neighbors)
        {
            Gizmos.DrawLine(gameObject.transform.position, neighbor.transform.position);
        }
    }
}
