using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATE
{
    NONE,
    IDLE,
    HELP,
    SELFTASK,
    PATHFIND,
    WALK,
    ARRIVED,
    WORK,
    COMPLETE,
    ABANDON,
    WANDER,
}

public class AIPhaseController : MonoBehaviour
{
    [Header("References")]
    public AIPathfindingController PathfindingController;
    public AIStatsController StatsController;
    public AIWorkController WorkController;

    private bool statInitiated, pathfindingInitiated, workInitiated;
    private bool moodRecovering = false;

    public STATE currentState
    {
        get { return state; }
        set
        {
            if (state != value)
            {
                previousState = state;
                state = value;
                if (_stateCheck == null)
                {
                    _stateCheck = StartCoroutine(CheckState());
                    AILogger.Log(gameObject.name, "STATE CHANGE: " + value);
                }
            }
        }
    }
    public int AffectMood
    {
        get => StatsController.Mood;
        set
        { 
            StatsController.Mood = value;
            StatsController.MoodAffect();
            MoodCheck(StatsController.Mood);
        }   
    }
    public int TasksCount
    {
        get => StatsController.GetTasksCount();
    }

    private STATE state = STATE.NONE;
    [HideInInspector] public STATE previousState = STATE.NONE;

    private Coroutine _stateCheck;

    private bool stop = false;
    private int moodbreakpoint = 0;
    

    public void INIT()
    {
        statInitiated = StatsController.INIT(this);
        if (!statInitiated) { Debug.LogWarning("No Stats Controller were found!"); return; };
        pathfindingInitiated = PathfindingController.INIT(this);
        if (!pathfindingInitiated) { Debug.LogWarning("No Pathfinding Controller were found!"); return; };
        workInitiated = WorkController.INIT(this);
        if (!workInitiated) { Debug.LogWarning("No Work Controller were found!"); return; }
        currentState = STATE.IDLE;
    }

    private IEnumerator CheckState()
    {
        if (stop) yield return null;
        switch (currentState)
        {
            case STATE.IDLE:
                yield return new WaitForSeconds(1f);
                DisableCoroutine();
                if (AffectMood <= moodbreakpoint)
                {
                    currentState = STATE.WANDER;
                    break;
                }
                WorkController.currentTask = StatsController.GetTask();
                currentState = STATE.PATHFIND;
                PathfindingController.StartPathing(WorkController.currentTask.Point);
                break;

            case STATE.ARRIVED:
                yield return new WaitForSeconds(1f);
                DisableCoroutine();
                currentState = STATE.WORK;
                WorkController.PerformTask();
                break;

            case STATE.COMPLETE:
                StatsController.TaskComplete(WorkController.currentTask);
                PathfindingController.SetPoint();
                WorkController.AbandonTask();
                WorkController.currentTask = null;
                if (TasksCount <= 0)
                {
                    currentState = STATE.NONE;
                    stop = true;
                    break;
                }
                yield return new WaitForSeconds(2f);
                DisableCoroutine();
                currentState = STATE.IDLE;
                break;

            case STATE.ABANDON:
                PathfindingController.SetPoint();
                WorkController.AbandonTask();
                WorkController.currentTask = null;
                yield return new WaitForSeconds(2f);
                DisableCoroutine();
                currentState = STATE.IDLE;
                break;

            case STATE.WANDER:
                PathfindingController.ClearPathing();
                yield return new WaitForSeconds(2f);
                DisableCoroutine();
                PathfindingController.StartPathing(GameManager.Instance.GetWanderPoint(gameObject));
                break;

            default:
                break;
        }
    }

    private void MoodCheck(int mood)
    {
        moodbreakpoint = Random.Range(5, 20);
        if (mood > moodbreakpoint || moodRecovering) return;
        if (currentState == STATE.WORK)
        {
            currentState = STATE.ABANDON; // Abandon task first before going wander mode
        }    
        if (currentState == STATE.WALK) // Prevent state going haywire
        {
            currentState = STATE.WANDER;
        }
    }

    private void DisableCoroutine()
    {
        if (_stateCheck != null)
        {
            StopCoroutine(_stateCheck);
            _stateCheck = null;
        }
    }

    public IEnumerator MoodRecovery()
    {
        moodRecovering = true;
        while (currentState == STATE.WANDER)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            AffectMood += Random.Range(8, 12);
        }
        moodRecovering = false;
    }

    public void CompletedTask()
    {
        currentState = STATE.COMPLETE;
    }

    public void GoalReached()
    {

    }

    public GeneralData GetGeneralData()
    {
        GeneralData generalData = StatsController.AIStats.GetGeneralData();
        return generalData;
    }

    public WorkData GetWorkData()
    {
        WorkData workData = StatsController.AIStats.GetWorkData();
        return workData;
    }
}
