using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public delegate void FunctionalStateMachine();

public class GameSystem : MonoBehaviour
{
    /* Saveデータ形式
     * 宝箱情報...unsigned intのビット演算式
     * みこちゃん...各スキルLv、経験値、アイテム所持数
     * 進行度...クリアしたエリア数
    */

    static GameSystem gameSystemInstance;
    public static GameSystem Instance => gameSystemInstance;

    public readonly WaitForSeconds QuarterSecond = new WaitForSeconds(0.25f);
    public readonly WaitForSeconds HalfSecond = new WaitForSeconds(0.5f);
    public readonly WaitForSeconds OneSecond = new WaitForSeconds(1f);
    public readonly WaitForSeconds TwoSecond = new WaitForSeconds(2f);
    public readonly WaitForSeconds ThreeSecond = new WaitForSeconds(3f);
    public readonly WaitForSeconds FourSecond = new WaitForSeconds(4f);
    public readonly WaitForSeconds FiveSecond = new WaitForSeconds(5f);

    bool movie;

    FunctionalStateMachine scene;

    [SerializeField] CircleGrayScaleEffect cgse = default;
    [SerializeField] Image darkness = default;
    [SerializeField] Image whiteness = default;

    public bool enemyStop;
    public bool Movie => movie;
    public bool Stop = false;
    public bool IsGameOver = true;
    public Image Darkness => darkness;
    public Image Whiteness => whiteness;

    void Awake()
    {
        gameSystemInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        darkness.CrossFadeAlpha(0, 0, true);
        darkness.gameObject.SetActive(true);
        TrainingSceneManager.Instance.IsOperational = true;
        Stop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TrainingSceneManager.Instance.DisplayTrainingScene();
        }
        /* //テスト実装
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (CameraManager.Instance.MainPostEffect.GetEffectActive(MyPostEffects.CIRCLE_GRAYSCALE_EFFECT) && enemyStop)
            {
                CameraManager.Instance.MainPostEffect.SetEffectActive(MyPostEffects.CIRCLE_GRAYSCALE_EFFECT, false);
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

        if (CameraManager.Instance.MainPostEffect.GetEffectActive(MyPostEffects.CIRCLE_GRAYSCALE_EFFECT))
        {
            cgse.radius += 0.05f;
            if (cgse.radius > 2f)
            {
                cgse.radius = 2f;
            }
        }
        */
        Mikochan.Instance.Act();
        AreaManager.Instance.Act();
        EventTextManager.Instance.Act();
    }
    
    void AreaScene()
    {
        Mikochan.Instance.Act();
        AreaManager.Instance.Act();
        EventTextManager.Instance.Act();
    }

    void AreaSelectScene()
    {

    }

    public void GameStop()
    {
        AreaManager.Instance.StopEnemy();
        Mikochan.Instance.Pause();
        Stop = true;
        //Time.timeScale = 0;
    }

    public void GameRestart()
    {
        AreaManager.Instance.RestartEnemy();
        Mikochan.Instance.Restart();
        Stop = false;
        //Time.timeScale = 1f;
    }

    public void GameOver()
    {
        TrainingSceneManager.Instance.IsOperational = false;
        IsGameOver = true;
        StartCoroutine("ToResult");
    }

    public void GameClear()
    {
        TrainingSceneManager.Instance.IsOperational = false;
        IsGameOver = false;
        StartCoroutine("ToResult");
    }

    IEnumerator ToResult()
    {
        if (IsGameOver)
        {
            darkness.CrossFadeAlpha(1.0f, 0.5f, true);
            yield return HalfSecond;
        }
        else
        {
            darkness.CrossFadeAlpha(1.0f, 5.0f, true);
            yield return FiveSecond;
        }
        SceneManager.LoadScene("Result");
    }

    //暗転やBOSSの登場シーンの制御をこれに
}
