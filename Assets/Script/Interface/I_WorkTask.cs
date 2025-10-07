using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_WorkTask 
{
    public void PerformTask(float value);
    public void EnableTask();
    public void AssignWorker(AIPhaseController controller, bool value);
    public Transform GetSpotPosition();
}
