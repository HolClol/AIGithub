using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkPlaceController : MonoBehaviour, I_WorkTask
{
    public List<Transform> spots = new List<Transform>();

    public bool Completed;
    public float TaskRequiredAmount;
    public float TaskProgress;
    public float BonusTimer;

    [SerializeField] private List<AIPhaseController> workers =  new List<AIPhaseController>();
    private int spotsTaken;

    public void AssignWorker(AIPhaseController controller, bool value)
    {
        if (workers.Contains(controller) && !value)
        {
            workers.RemoveAll(x => x == controller);
            workers.RemoveAll(x => !x);
            spotsTaken--;
        } 
        else if (value)
        {
            workers.Add(controller);
            spotsTaken++;
        }
            
    }

    public void EnableTask()
    {
        throw new System.NotImplementedException();
    }

    public Transform GetSpotPosition()
    {
        return spots[spotsTaken];
    }

    public void PerformTask(float value)
    {
        TaskProgress += value;
        if (TaskProgress >= TaskRequiredAmount)
        {
            var list = new List<AIPhaseController>(workers);
            CompleteTask(list);
        }
    }

    public void CompleteTask(List<AIPhaseController> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].CompletedTask();
        }
        GameManager.Instance.TaskComplete();
        GameManager.Instance.IncreaseTimer(BonusTimer);
        Completed = true;
    }
}
