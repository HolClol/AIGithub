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
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    public void UpdateTimer(float timer)
    {
        txtTimer.text = timer.ToString();
    }
}
