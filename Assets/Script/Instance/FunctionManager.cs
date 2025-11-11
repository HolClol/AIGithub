using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionManager : MonoBehaviour
{
    public static FunctionManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    public bool ChanceGenerator(float chance)
    {
        float check = UnityEngine.Random.Range(0f, 100f);
        if (chance * 100f > check)
        {
            return true;
        }

        return false;
    }

    public string FormatSecondToStringTime(float seconds)
    {
        string value;
        TimeSpan time;
        if (seconds >= 3600f)
        {
            time = TimeSpan.FromSeconds(seconds);
            value = time.ToString(@"h\:mm\:ss");
        }
        else
        {
            time = TimeSpan.FromSeconds(seconds);
            value = time.ToString(@"mm\:ss");
        }
        return value;
    }
}
