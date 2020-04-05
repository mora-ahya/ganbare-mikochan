using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private static StageManager smInstance;

    public static StageManager Instance => smInstance;

    [SerializeField] GameObject currentStage = default;
    Stage stage;
    GameObject preStage = null;
    GameObject tmp;

    void Awake()
    {
        smInstance = this;
    }

    void Start()
    {
        if (currentStage != default)
        {
            stage = currentStage.GetComponent<Stage>();
            stage.Initialize();
        }

    }

    public void Act()
    {
        stage.Act();
    }

    public void Transition(GameObject nextStage)
    {
        if (preStage == null)
        {
            preStage = currentStage;
            currentStage = Instantiate(nextStage);
            currentStage.name = nextStage.name;
            preStage.SetActive(false);
            return;
        }

        if (preStage.name == nextStage.name)
        {
            tmp = currentStage;
            currentStage = preStage;
            preStage = tmp;
            preStage.SetActive(false);
            currentStage.SetActive(true);
            currentStage.GetComponent<Stage>().Initialize();
            return;
        }

        Destroy(preStage);
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        preStage = currentStage;
        currentStage = Instantiate(nextStage);
        currentStage.name = nextStage.name;
        preStage.SetActive(false);

    }
}
