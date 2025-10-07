using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Vector3 position;
    public List<GameObject> Neighbors; // Exist to be drawn on gizmos
    [HideInInspector] public List<Waypoint> Neighbours; // Real table to be taken from

    protected virtual void Awake()
    {
        position = transform.position;
        foreach (var neighbor in Neighbors) 
        { 
            Neighbours.Add(neighbor.GetComponent<Waypoint>());
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (Neighbors.Count <= 0) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, 0.5f);
        foreach (var neighbor in Neighbors)
        {
            Gizmos.DrawLine(gameObject.transform.position, neighbor.transform.position);
        }
    }
}
