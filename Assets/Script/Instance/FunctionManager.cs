using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionManager : MonoBehaviour
{
    public static FunctionManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }
    public bool ChanceGenerator(float chance)
    {
        float check = Random.Range(0f, 100f);
        if (chance * 100f > check)
        {
            return true;
        }

        return false;
    }

}
