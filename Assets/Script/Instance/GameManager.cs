using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public WaypointGraph WaypointGraph;
    public List<CharacteristicTraits> TraitsTable;
    public List<WorkTaskClass> WorkTasksTable;
    public List<AIPhaseController> AisTable;
    public int TasksGiven = 0;

    private List<WorkTaskClass> tasksGiven = new List<WorkTaskClass>();
    private List<Waypoint_Endpoint> endPoints = new List<Waypoint_Endpoint>();

    private float timer = 150f;
    private int completedCount;

    private void Start()
    {
        Instance = this;

        WaypointGraph.INIT();

        tasksGiven = new List<WorkTaskClass>(WorkTasksTable);
        endPoints = new List<Waypoint_Endpoint>(WaypointGraph.Endpoints);

        AssignEndpointForTask();

        foreach (AIPhaseController AI in AisTable)
        {
            AI.INIT();
        }

        StartCoroutine(TimerStart());
    }

    public void TaskComplete()
    {
        completedCount++;
        if (completedCount >= TasksGiven)
        {
            Debug.Log("Game Win");
        }
    }

    public void IncreaseTimer(float time)
    {
        timer += time;
        UIManager.Instance.UpdateTimer(timer);
    }

    #region GameGeneration
    public CharacteristicTraits GetRandomTrait()
    {
        return TraitsTable[Random.Range(0, TraitsTable.Count - 1)];
    }

    public WorkTaskClass GetRandomTask()
    {
        var task = tasksGiven[Random.Range(0, tasksGiven.Count)];
        tasksGiven.Remove(task); // Task will be removed to avoid duplicates
        TasksGiven++;
        return task;
    }

    public Waypoint_Wanderpoint GetWanderPoint(GameObject owner)
    {
        float closestdist = 200f;
        Waypoint_Wanderpoint chosenpoint = WaypointGraph.Wanderpoints[Random.Range(0, WaypointGraph.Wanderpoints.Count)]; //Set to random at first
        //Return the closest wander spot
        foreach (var pos in WaypointGraph.Wanderpoints)
        {
            float dist = Vector3.Distance(pos.gameObject.transform.position, owner.transform.position);
            if (dist < closestdist)
            {
                closestdist = dist;
                chosenpoint = pos;
            }
        }
        return chosenpoint;
    }

    private void AssignEndpointForTask()
    {
        foreach(WorkTaskClass task in tasksGiven)
        {
            var index = Random.Range(0, endPoints.Count);
            task.Point = endPoints[index];
            task.WorkPlaceSpot = endPoints[index].controller;

            int multi = Random.Range(1, 4);
            endPoints[index].controller.BonusTimer = multi * 10f;
            endPoints[index].controller.TaskRequiredAmount = task.TaskRequired + (multi * 10f);
            endPoints.Remove(endPoints[index]);
        }
    }

    private IEnumerator TimerStart()
    {
        while (timer > 0f)
        {
            timer -= 1f;
            UIManager.Instance.UpdateTimer(timer);
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Game lose");
    }
    #endregion
}
