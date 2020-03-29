using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    static GameSystem gameSystemInstance;
    public static GameSystem Instance => gameSystemInstance;
    public static bool stop = false;
    public static bool gameOver = true;

    public bool enemyStop;

    bool movie;
    bool canOpenTS = true;
    float waitTime;

    public bool Movie => movie;
    public Image Darkness => darkness;
    public Image Whiteness => whiteness;

    [SerializeField] Mikochan mikochan = default;
    [SerializeField] TrainingSceneManager tsm = default;
    [SerializeField] EventTextManager etm = default;
    [SerializeField] MyPostEffects mpe = default;
    [SerializeField] HowlManager hm = default;
    [SerializeField] CircleGrayScaleEffect cgse = default;
    [SerializeField] Image darkness = default;
    [SerializeField] Image whiteness = default;

    void Awake()
    {
        gameSystemInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        darkness.CrossFadeAlpha(0, 0, true);
        darkness.gameObject.SetActive(true);
        stop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canOpenTS && !etm.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (stop)
            {
                mpe.SetEffectActive(MyPostEffects.GAUSSIANBLUR_EFFECT, false);
                stop = false;
                Time.timeScale = 1.0f;
                if (tsm.IsRunning)
                {
                    tsm.ParticleOff();
                }
                tsm.gameObject.SetActive(false);
            }
            else
            {
                mpe.SetEffectActive(MyPostEffects.GAUSSIANBLUR_EFFECT, true);
                stop = true;
                Time.timeScale = 0;
                tsm.gameObject.SetActive(true);
            }
        }
        /* //テスト実装
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (mpe.GetEffectActive(MyPostEffects.CIRCLE_GRAYSCALE_EFFECT) && enemyStop)
            {
                mpe.SetEffectActive(MyPostEffects.CIRCLE_GRAYSCALE_EFFECT, false);
                enemyStop = false;
            }
            else if (!enemyStop)
            {
                cgse.radius = 0;
                cgse.startPoint = new Vector2(0.5f, 0.3f);
                Mikochan.Instance.TimeStop();
                enemyStop = true;
            }
        }

        if (mpe.GetEffectActive(MyPostEffects.CIRCLE_GRAYSCALE_EFFECT))
        {
            cgse.radius += 0.05f;
            if (cgse.radius > 2f)
            {
                cgse.radius = 2f;
            }
        }
        */
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
