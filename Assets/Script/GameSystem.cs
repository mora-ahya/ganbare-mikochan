using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public static bool stop = false;
    public static bool gameOver = true;

    private bool movie;
    bool canOpenTS = true;
    float waitTime;

    public bool Movie => movie;

    [SerializeField] Mikochan mikochan;
    [SerializeField] TrainingSceneManager tsm;
    [SerializeField] EventTextManager etm;
    [SerializeField] HowlManager hm;
    [SerializeField] Image darkness;

    // Start is called before the first frame update
    void Start()
    {
        darkness.CrossFadeAlpha(0, 0, true);
        darkness.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (canOpenTS && !etm.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (stop)
            {
                stop = false;
                Time.timeScale = 1.0f;
                if (tsm.IsRunning)
                {
                    tsm.ParticleOff();
                }
                tsm.gameObject.SetActive(false);
            } else
            {
                stop = true;
                Time.timeScale = 0;
                tsm.gameObject.SetActive(true);
            }
        }
        //Debug.Log(Input.mousePosition);
    }

    public void GameOver()
    {
        canOpenTS = false;
        gameOver = true;
        waitTime = 0.5f;
        StartCoroutine("ToResult");
        Time.timeScale = 0;
    }

    public void GameClear()
    {
        canOpenTS = false;
        gameOver = false;
        waitTime = 5.0f;
        StartCoroutine("ToResult");
    }

    IEnumerator ToResult()
    {
        darkness.CrossFadeAlpha(1.0f, waitTime, true);
        yield return new WaitForSecondsRealtime(waitTime);
        SceneManager.LoadScene("Result");
    }

    //暗転やBOSSの登場シーンの制御をこれに
}
