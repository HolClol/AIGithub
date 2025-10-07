using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWorkController : AIComponents
{
    public WorkData AIWorkData
    {
        get { return owner.GetWorkData(); }
    }
    public int TasksCount
    {
        get => owner.TasksCount;
    }

    public WorkTaskClass currentTask;
    private WorkPlaceController currentWork;

    public override bool INIT(AIPhaseController owner)
    {
        initialized = true;
        this.owner = owner;

        return true;    
    }

    public void PerformTask()
    {
        currentWork = currentTask.WorkPlaceSpot;
        currentWork.AssignWorker(owner, true);
        owner.transform.position = currentWork.GetSpotPosition().position;
        StartCoroutine(TaskPerform());
    }

    public void AbandonTask()
    {
        currentWork.AssignWorker(owner, false);
    }

    private IEnumerator TaskPerform()
    {
        while (owner.currentState == STATE.WORK)
        {
            yield return new WaitForSeconds(GetDelayTimer());
            // Work Abandon
            if (FunctionManager.Instance.ChanceGenerator(AIWorkData.WorkAbandonChance))
                owner.currentState = STATE.ABANDON;

            // Work Bonus
            if (FunctionManager.Instance.ChanceGenerator(AIWorkData.WorkBonusChance))
            {
                owner.AffectMood -= (int)(AIWorkData.WorkQuality * 0.1f);
                currentWork.PerformTask(AIWorkData.WorkQuality * 2f);
                continue;
            }

            // Work Fail
            if (FunctionManager.Instance.ChanceGenerator(AIWorkData.WorkFailChance))
            {
                owner.AffectMood -= (int)(AIWorkData.WorkQuality);
                continue;
            }

            owner.AffectMood -= (int)(AIWorkData.WorkQuality * 0.3f);
            currentWork.PerformTask(AIWorkData.WorkQuality * 1f);
        }
    }

    private float GetDelayTimer()
    {
        float random = Random.Range(12f, 15f);
        float count = Mathf.Max(random - AIWorkData.WorkSpeed, 1f);
        return count;
    }

    
}
