using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private static StageManager smInstance;

    public static StageManager Instance => smInstance;

    [SerializeField] private GameObject currentStage;
    private GameObject preStage = null;
    private GameObject tmp;

    void Awake()
    {
        smInstance = this;
    }

    public void Transition(GameObject nextStage)
    {
        if (preStage == null)
        {
            preStage = currentStage;
            currentStage = Instantiate(nextStage);
            currentStage.name = nextStage.name;
            preStage.SetActive(false);
        }
        else if (preStage.name == nextStage.name)
        {
            tmp = currentStage;
            currentStage = preStage;
            preStage = tmp;
            preStage.SetActive(false);
            currentStage.SetActive(true);
            currentStage.GetComponent<Stage>().Reset();
        }
        else
        {
            Destroy(preStage);
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
            preStage = currentStage;
            currentStage = Instantiate(nextStage);
            currentStage.name = nextStage.name;
            preStage.SetActive(false);
        }
    }
}
