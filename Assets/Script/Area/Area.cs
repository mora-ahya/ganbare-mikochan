using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    [SerializeField] Treasure[] treasureInArea = default;
    [SerializeField] Stage initStage = default;
    [SerializeField] Stage currentStage = default;

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
