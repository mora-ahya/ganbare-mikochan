using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    private static AreaManager areaManagerInstance;

    public static AreaManager Instance => areaManagerInstance;

    [SerializeField] GameObject currentAreaObject = default;
    Area currentArea;
    GameObject preArea = null;
    GameObject tmp;

    void Awake()
    {
        areaManagerInstance = this;
    }

    void Start()
    {
        if (currentAreaObject != default)
        {
            currentArea = currentAreaObject.GetComponent<Area>();
            currentArea.Initialize();
        }

    }

    public void Act()
    {
        currentArea.Act();
    }

    public void SetArea(GameObject otherArea)
    {
        if (preArea == null)
        {
            preArea = currentAreaObject;
            currentAreaObject = Instantiate(otherArea);
            currentAreaObject.name = otherArea.name;
            preArea.SetActive(false);
            return;
        }

        if (preArea.name == otherArea.name)
        {
            tmp = currentAreaObject;
            currentAreaObject = preArea;
            preArea = tmp;
            preArea.SetActive(false);
            currentAreaObject.SetActive(true);
            currentAreaObject.GetComponent<Stage>().Initialize();
            return;
        }

        Destroy(preArea);
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        preArea = currentAreaObject;
        currentAreaObject = Instantiate(otherArea);
        currentAreaObject.name = otherArea.name;
        preArea.SetActive(false);

    }

    public int GetCurrentAreaNumber()
    {
        return currentArea.AreaNumber;
    }

    public void StageTransition(Stage nextStage)
    {
        currentArea.Transition(nextStage);
    }

    public void StopEnemy()
    {
        currentArea.StopEnemy();
    }

    public void RestartEnemy()
    {
        currentArea.RestartEnemy();
    }
}
