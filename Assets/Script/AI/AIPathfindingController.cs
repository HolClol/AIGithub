using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class AIPathfindingController : AIComponents
{
    public Waypoint StartPoint;
    public Waypoint EndPoint;
    public Waypoint CurrentPoint;

    public GeneralData AIGeneralData
    {
        get { return owner.GetGeneralData(); }
    }

    private List<Waypoint> _waypoints = new List<Waypoint>(); // Cache the waypoints as they cannot change in runtime
    [SerializeField] private List<Waypoint> _realPath = new List<Waypoint>();

    private Waypoint_Wanderpoint _wanderPoint;
    [SerializeField] private Coroutine _pathfinding;

    private bool wandering = false;
    [SerializeField] private int pathIndex = 0;

    public override bool INIT(AIPhaseController owner)
    {
        initialized = true;
        this.owner = owner;

        _waypoints = WaypointGraph.Instance.GetWaypoints();
        return initialized;
    }

    public void StartPathing(Waypoint endpoint)
    {
        if (owner.currentState != STATE.WANDER)
            owner.currentState = STATE.WALK;
        if (endpoint.GetType() == typeof(Waypoint_Wanderpoint))
        {
            _wanderPoint = endpoint as Waypoint_Wanderpoint;
            StartCoroutine(DisableWander(_wanderPoint));
        }
           
        FindClosestWaypoint();
        EndPoint = endpoint;
        _realPath = FindPath(StartPoint, EndPoint);
        FireCoroutine(true);

    }

    public void SetPoint()
    {
        if (CurrentPoint != null)
            owner.transform.position = CurrentPoint.position;
        ClearPathing();
    }

    public void ClearPathing()
    {
        CurrentPoint = null;
        StartPoint = null;
        EndPoint = null;
        _realPath.Clear();
        pathIndex = 0;
    }

    private IEnumerator ProcessUpdate()
    {
        while (owner.currentState != STATE.NONE)
        {
            if (!initialized || _realPath == null) yield return new WaitForSeconds(0.05f);

            if (owner.currentState == STATE.WALK && pathIndex >= _realPath.Count)
            {
                owner.currentState = STATE.ARRIVED;
                FireCoroutine(false);
            }

            if (FunctionManager.Instance.ChanceGenerator(AIGeneralData.WanderOffChance) && owner.currentState != STATE.WANDER)
            {
                owner.currentState = STATE.WANDER;
                FireCoroutine(false);
            }

            // Fire a burst of movement
            Coroutine Run = StartCoroutine(BurstMovement(Random.Range(0.5f, 1.5f)));
            yield return Run;

            yield return new WaitForSeconds(Random.Range(0.25f, 1f));
        }
    }

    private void FireCoroutine(bool value)
    {
        if (value)
        {
            if (_pathfinding == null)
            {
                _pathfinding = StartCoroutine(ProcessUpdate());
            }
        }
        else
        {
            if (_pathfinding != null)
            {
                StopCoroutine(_pathfinding);
                _pathfinding = null;
            }
        }          
    }

    private IEnumerator BurstMovement(float duration)
    {
        if (owner.currentState != STATE.WANDER)
        {
            owner.AffectMood -= Random.Range(1, 3);
        }     

        float timer = duration;
        while (timer > 0f && pathIndex < _realPath.Count)
        {
            // Move towards the current waypoint
            Vector3 targetPos = _realPath[pathIndex].transform.position;
            float step = (AIGeneralData.MoveSpeed * 0.6f) * Time.deltaTime;
            owner.transform.position = Vector2.MoveTowards(owner.transform.position, targetPos, step);
            timer -= Time.deltaTime /* (* MoveSpeed) */;

            // If we've reached this waypoint, advance to the next
            if (Vector2.Distance(owner.transform.position, targetPos) < 0.1f)
            {
                CurrentPoint = _realPath[pathIndex];
                if (wandering)
                    pathIndex = Random.Range(0, _realPath.Count);
                else
                    pathIndex++;
                
                yield break;
            }
            yield return null;
        }

        if (pathIndex >= _realPath.Count && _realPath.Count > 0)
        {
            if (!wandering && owner.currentState == STATE.WANDER && _wanderPoint != null)
            {
                SetWanderSpot(_wanderPoint);
            }
        }
    }

    #region WANDER
    public IEnumerator DisableWander(Waypoint point)
    {
        while (!wandering)
            yield return new WaitForSeconds(1f);

        StartCoroutine(owner.MoodRecovery());
        float mintimer = Mathf.Clamp(owner.AffectMood * 0.1f, 0f, 10f);
        float maxtimer = Mathf.Clamp(owner.AffectMood * 0.15f, 0f, 15f);
        yield return new WaitForSeconds(Random.Range(10f - mintimer, 20f - maxtimer));
        wandering = false;
        CurrentPoint = point;
        SetPoint();
        _wanderPoint = null;
        owner.currentState = STATE.IDLE;
    }

    private void SetWanderSpot(Waypoint_Wanderpoint wanderspot)
    {
        FireCoroutine(false);
        var list = wanderspot.WanderingSpots;
        wandering = true;
        _realPath = new List<Waypoint>(list);
        pathIndex = Random.Range(0, list.Count);
        owner.transform.position = _realPath[pathIndex].position;
        FireCoroutine(true);
    }
    #endregion

    #region DEBUGGING
    // If the start point is not set (Due to just spawned or for debugging), it will automatically find the nearest waypoint
    private void FindClosestWaypoint()
    {
        float olddist = 100f;
        if (StartPoint != null)
        {
            return;
        }

        foreach (Waypoint waypoint in _waypoints)
        {
            if (Vector2.Distance(transform.position, waypoint.position) < olddist)
            {
                olddist = Vector2.Distance(transform.position, waypoint.position);
                StartPoint = waypoint;
                CurrentPoint = waypoint;
            }
        }
    }
    #endregion

    #region PATHFINDING 
    // ======================== // PATHFINDING FUNCTIONS \\ ========================

    // A* search over manually linked waypoints
    private List<Waypoint> FindPath(Waypoint start, Waypoint goal)
    {
        var openSet = new List<Waypoint> { start }; // Save all waypoints 
        var cameFrom = new Dictionary<Waypoint, Waypoint>(); // Best waypoint from previous cleared waypoint

        var gScore = new Dictionary<Waypoint, float>(); // The cheapest / closest waypoints that has been found
        var fScore = new Dictionary<Waypoint, float>(); // Determine the best waypoint for the goal
        foreach (var wp in FindObjectsOfType<Waypoint>())
        {
            gScore[wp] = float.PositiveInfinity;
            fScore[wp] = float.PositiveInfinity;
        }

        gScore[start] = 0f;
        fScore[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            // Pick node with lowest fScore
            Waypoint current = openSet[0];
            float bestF = fScore[current];
            foreach (var node in openSet)
            {
                if (fScore[node] < bestF)
                {
                    bestF = fScore[node];
                    current = node;
                }
            }

            // If path reached to goal, construct the path
            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);

            // Examining each neighbor
            foreach (var neighbor in current.Neighbours)
            {
                if (neighbor == null) continue;

                float tentativeG = gScore[current] + Vector3.Distance(
                    current.transform.position,
                    neighbor.transform.position);

                // If it is the better path then record it
                if (tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    // Make sure neighbor is within the set
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // No path found
        return null;
    }

    private float Heuristic(Waypoint a, Waypoint b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    private List<Waypoint> ReconstructPath(Dictionary<Waypoint, Waypoint> cameFrom, Waypoint current)
    {
        var totalPath = new List<Waypoint> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }
    #endregion
}
