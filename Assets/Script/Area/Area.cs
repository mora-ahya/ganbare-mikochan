using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    [SerializeField] Stage initStage = default;
    [SerializeField] Stage currentStage = default;
    [SerializeField] int areaNumber = default;

    public int AreaNumber => areaNumber;

    public void Initialize()
    {
        currentStage = initStage;
        currentStage.gameObject.SetActive(true);
        currentStage.Initialize();
    }

    public void Act()
    {
        currentStage.Act();
    }

    public void Transition(Stage nextStage)
    {
        currentStage.gameObject.SetActive(false);
        currentStage = nextStage;
        currentStage.gameObject.SetActive(true);
        currentStage.Initialize();
    }

    public void StopEnemy()
    {
        currentStage.StopEnemy();
    }

    public void RestartEnemy()
    {
        currentStage.RestartEnemy();
    }
}
