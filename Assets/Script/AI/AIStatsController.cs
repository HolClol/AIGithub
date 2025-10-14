using System.Collections.Generic;
using UnityEngine;

public class AIStatsController : AIComponents
{
    private readonly int[] thresholds = { 80, 60, 40, 20 };

    public AiStatsClass AIStats;
    [Header("In Game Stats Display")]
    [Range(0, 100)] public int mood = 100;
    public List<CharacteristicTraits> AITraits;
    public List<WorkTaskClass> WorkTasks;
    public int Mood
    {
        get => mood;
        set => mood = Mathf.Clamp(value, 0, 100);
    }

    private AiStatsClass BaseAIStats = new AiStatsClass { }; // For when stats manipulation occurs, this object is used to reset stats to normal

    public override bool INIT(AIPhaseController owner)
    {
        initialized = true;
        this.owner = owner;

        BaseAIStats.ReconfigStat(AIStats, 1f);
        SetRandomTraits();
        SetStats();
        SetTasks();

        return initialized;
    }

    #region Set Stats
    private void SetRandomTraits()
    {
        int numbOfTraits = UnityEngine.Random.Range(4, 6);
        for (int i = 0; i < numbOfTraits; i++)
        {
            CharacteristicTraits trait = GameManager.Instance.GetRandomTrait();
            if (!AITraits.Contains(trait) && !AITraits.Contains(trait.OppositeTrait))
                AITraits.Add(trait); 
        }
    }

    private void SetStats()
    {   
        foreach (CharacteristicTraits trait in AITraits)
        {
            AIStats.TraitReconfigStats(trait.Stats);
            BaseAIStats.TraitReconfigStats(trait.Stats);
        }
    }
    
    private void SetTasks()
    {
        int numbOfTasks = 2;
        for (int i = 0; i < numbOfTasks; i++)
        {
            WorkTaskClass task = GameManager.Instance.GetRandomTask();
            if (task != null)
                WorkTasks.Add(task);
        }
    }
    #endregion
    #region Tasks Stat

    public int GetTasksCount()
    {
        return WorkTasks.Count;
    }

    // Get tasks that was given from the AI, prefer tasks that are closer distance or almost finished
    public WorkTaskClass GetTask()
    {
        int coinroll = Random.Range(1, 2);
        float highestscore = 0f;
        float nearestdist = 200f;
        WorkTaskClass selectedtask = WorkTasks[Random.Range(0, WorkTasks.Count - 1)];
        foreach (WorkTaskClass task in WorkTasks) 
        {
            Vector3 taskpos = task.Point.gameObject.transform.position;
            float score = task.WorkPlaceSpot.TaskProgress;
            float dist = Vector3.Distance(owner.transform.position, taskpos);

            
            if (coinroll == 1)
            {
                if (task.WorkPlaceSpot.TaskProgress > 0)
                {
                    selectedtask = task;
                    highestscore = score;
                }
            }
            else if (coinroll == 2)
            {
                if (dist < nearestdist)
                {
                    selectedtask = task;
                    nearestdist = dist;
                }
            }
            
        }
        return selectedtask;
    }

    public void TaskComplete(WorkTaskClass task)
    {
        WorkTasks.Remove(task);
    }
    #endregion

    public void MoodAffect()
    {
        int crossed = 0;
        for (int i = 0; i < thresholds.Length; i++)
            if (Mood <= thresholds[i])
                crossed++;

        float moodMulti = 1f - 0.1f * crossed;
        AIStats.ReconfigStat(BaseAIStats, moodMulti);
    }
}
