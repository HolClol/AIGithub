using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Task
{
    LogChopping,
    MetalProduction,
    DataTransmition,
    OfficeWorking,
    QualityChecking,
    PaperMarking,
    SteelMelting,
    ScriptWriting
}

[Serializable] public class WorkTaskClass  
{
    public Task Task;
    public Waypoint_Endpoint Point;
    public WorkPlaceController WorkPlaceSpot;
    public float TaskRequired;
}

[CreateAssetMenu(fileName = "WorkTask", menuName = "Scriptable Object/Work Task", order = 0)]
public class WorkTaskSO : ScriptableObject
{
    public WorkTaskClass WorkTask;
}
