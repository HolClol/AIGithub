using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI txtTimer;

    void Start()
    {
        Instance = this;
    }

    public void UpdateTimer(float timer)
    {
        string txt = FunctionManager.Instance.FormatSecondToStringTime(timer);
        txtTimer.text = txt;
    }
}
