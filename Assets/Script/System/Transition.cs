using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    [SerializeField] Image panel = default;

    bool isRunning;
    float endTime;
    float endDuration;
    float charaTime;

    public bool IsRunning => isRunning;

    public void Init()
    {
        gameObject.SetActive(true);
        panel.CrossFadeAlpha(0f, 0f, true);
    }

    public void Act()
    {
        charaTime += Time.deltaTime;
        if (charaTime <= endTime)
            return;

        panel.CrossFadeAlpha(0f, endDuration, true);
        isRunning = false;
    }

    public void HealEffect()
    {
        if (isRunning)
            panel.CrossFadeAlpha(0f, 0f, true);

        panel.color = Color.green;
        panel.CrossFadeAlpha(0.5f, 0.5f, true);
        isRunning = true;
        endTime = 0.5f;
        endDuration = 0.5f;
        charaTime = 0f;
    }
}
