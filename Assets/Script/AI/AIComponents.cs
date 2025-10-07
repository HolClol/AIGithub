using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponents : MonoBehaviour
{
    protected AIPhaseController owner;
    protected bool initialized = false;
    public virtual bool INIT(AIPhaseController owner)
    {
        return false;
    }
}
