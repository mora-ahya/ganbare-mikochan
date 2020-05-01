using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSceneManager : MonoBehaviour
{
    static TrainingSceneManager trainingSceneManagerInstance;
    public static TrainingSceneManager Instance => trainingSceneManagerInstance;

    readonly float ohudaTop = 424.5f;
    readonly float ohudaSpace = 70f;
    readonly float buttonLeft = 525f;
    readonly float buttonSpace = 150f;
    readonly int maxLv = 25;
    readonly int offset = 5;
    readonly string[] explains = new string[] {
        "1回ごとに神様領域が少し広くなります。\n5回ごとに神様に関するスペシャルなスキルを獲得できます。",//kamiAreaButton
        "1回ごとにHPが5増加します。\n5回ごとにHPに関するスペシャルなスキルを獲得できます。",//hpButton
        "1回だけでは何も得られませんが、5回ごとにとてもスペシャルなスキルを獲得できます。",//verySpecialButton
        "???",
        "体力が10増加します",//hpButton
        "体力が20増加します",//hpButton   [5]
        "体力が30増加します",//hpButton
        "体力が40増加します",//hpButton
        "体力が50増加します",//hpButton
        "神様の攻撃力が1増加します。",//kamiAreaButton:1,3
        "神様の加護により受けるダメージが10%減少します。",//kamiAreaButton:2,4    [10]
        "神様の攻撃力が2増加します。",//kamiAreaButton:5
        "被ダメージ時のノックバックが軽減されます。",//verySpecialButton:1,4
        "未定",//verySpecialButton:2
        "神様の加護により受けるダメージが30%軽減されます。",//verySpecialButton:3
        "被ダメージ時の無敵時間が長くなります。"//verySpecialButton:5
    };

    [SerializeField] Image[] ohudas = default;
    [SerializeField] Button[] buttons = default;
    [SerializeField] GameObject pConv = default;
    [SerializeField] GameObject pDiver = default;
    [SerializeField] ParticleSystem psConv = default;
    [SerializeField] ParticleSystem psDiver = default;
    [SerializeField] Text tExp = default;
    [SerializeField] Text explain = default;

    ParticleSystem.MainModule psm;
    Image ohuda;
    Mikochan miko;
    bool isRunning = false;
    bool isOperational = true;
    readonly WaitForSecondsRealtime w = new WaitForSecondsRealtime(0.05f);

    int kamiAreaLv = 1;//save対象
    int hpLv = 1;//save対象
    int specialLv = 1;//save対象
    int necessaryExp = 50;
    int tmp;
    float endAmount;

    public bool IsOperational
    {
        get
        {
            return isOperational;
        }

        set
        {
            isOperational = value;
        }
    }

    void Awake()
    {
        trainingSceneManagerInstance = this;
        gameObject.SetActive(false);
    }

    void Start()
    {
        //particle.transform.position = particle.transform.TransformPoint(new Vector3(100, 175, 0));
        tExp.text = "必要神力: " + necessaryExp.ToString() + "P";
        miko = Mikochan.Instance;
    }

    public void DisplayTrainingScene()
    {
        if (!isOperational)
            return;

        if (GameSystem.Instance.Stop)
        {
            CameraManager.Instance.MainPostEffect.SetEffectActive(MyPostEffects.GAUSSIANBLUR_EFFECT, false);
            GameSystem.Instance.GameRestart();
            if (isRunning)
            {
                ParticleOff();
            }
            gameObject.SetActive(false);
            return;
        }

        CameraManager.Instance.MainPostEffect.SetEffectActive(MyPostEffects.GAUSSIANBLUR_EFFECT, true);
        GameSystem.Instance.GameStop();
        gameObject.SetActive(true);

    }

    void IncreaseNecessaryExp()
    {
        miko.GetExp(-necessaryExp);
        if (kamiAreaLv + hpLv + specialLv - 3 == 5)
        {
            necessaryExp = 150;
        }
        else if (kamiAreaLv + hpLv + specialLv - 3 == 15)
        {
            necessaryExp = 500;
        }
        else if (kamiAreaLv + hpLv + specialLv - 3 == 25)
        {
            necessaryExp = 1350;
        }
        else if (kamiAreaLv + hpLv + specialLv - 3 >= 50)
        {
            necessaryExp = 15000;
        }
        else
        {
            necessaryExp = (int)(necessaryExp * 1.1);
        }
        tExp.text = "必要神力: " + necessaryExp.ToString() + "P";
    }

    void ButtonSizeReset()
    {
        foreach(Button button in buttons)
        {
            if (button.gameObject.transform.localScale.x != 1)
            {
                button.gameObject.transform.localScale = Vector3.one;
            }
        }
    }

    public void PushedButton0()
    {
        if (!isRunning && kamiAreaLv <= maxLv && miko.Exp >= necessaryExp)
        {
            //buttons[0].interactable = false;
            ohuda = ohudas[(int)((kamiAreaLv - 1) / offset)];
            isRunning = true;
            miko.ExtendKamiArea();
            psm = psConv.main;
            psm.startColor = Color.yellow;
            psm = psDiver.main;
            psm.startColor = Color.yellow;
            pConv.transform.position = buttons[0].gameObject.transform.position;
            pDiver.transform.position = ohuda.transform.position;
            StartCoroutine("IncreaseFullAmount");
            if (kamiAreaLv % 5 == 0)
            {
                miko.Special(kamiAreaLv / offset + offset);
            }
            if (kamiAreaLv == maxLv)
            {
                buttons[0].interactable = false;
            }
            else
            {
                kamiAreaLv++;
            }
            IncreaseNecessaryExp();
        }
    }

    public void PushedButton1()
    {
        if (!isRunning && hpLv <= maxLv && miko.Exp >= necessaryExp)
        {
            ohuda = ohudas[(int)((hpLv - 1) / offset) + offset];
            isRunning = true;
            miko.IncreaseMikochanHP(5);
            psm = psConv.main;
            psm.startColor = Color.green;
            psm = psDiver.main;
            psm.startColor = Color.green;
            pConv.transform.position = buttons[1].gameObject.transform.position;
            pDiver.transform.position = ohuda.transform.position;
            StartCoroutine("IncreaseFullAmount");
            if (hpLv % 5 == 0)
            {
                miko.Special(hpLv / 5);
            }
            if (hpLv == maxLv)
            {
                buttons[1].interactable = false;
            }
            else
            {
                hpLv++;
            }
            IncreaseNecessaryExp();
        }
    }

    public void PushedButton2()
    {
        if (!isRunning && specialLv <= maxLv && miko.Exp >= necessaryExp)
        {
            ohuda = ohudas[(int)((specialLv - 1) / 5) + offset * 2];
            isRunning = true;
            psm = psConv.main;
            psm.startColor = Color.red;
            psm = psDiver.main;
            psm.startColor = Color.red;
            pConv.transform.position = buttons[2].gameObject.transform.position;
            pDiver.transform.position = ohuda.transform.position;
            StartCoroutine("IncreaseFullAmount");
            if (specialLv % 5 == 0)
            {
                miko.Special(specialLv / 5 + offset * 2);
            }
            if (specialLv == maxLv)
            {
                buttons[2].interactable = false;
            }
            else
            {
                specialLv++;
            }
            IncreaseNecessaryExp();
        }
    }

    public void ParticleOff()
    {
        pConv.SetActive(false);
        pDiver.SetActive(false);
        isRunning = false;
        ohuda.fillAmount = endAmount;
        ButtonSizeReset();
    }
    public void ExplainHidden()
    {
        explain.text = "";
    }

    public void ExplainHPSpecial()
    {
        tmp = (int)((ohudaTop - Input.mousePosition.y) / ohudaSpace);
        if ((hpLv - 1) / 5 > tmp || hpLv == 25)
        {
            explain.text = explains[tmp + 4];
        }
        else
        {
            explain.text = explains[3];
        }
    }

    public void ExplainKamiSpecial()
    {
        tmp = (int)((ohudaTop - Input.mousePosition.y) / ohudaSpace);
        if ((kamiAreaLv - 1) / 5 <= tmp && kamiAreaLv != 25)
        {
            explain.text = explains[3];
        }
        else if (tmp == 4)
        {
            explain.text = explains[11];
        }
        else if (tmp % 2 == 1 && tmp != 5)
        {
            explain.text = explains[10];
        }
        else
        {
            explain.text = explains[9];
        }
    }

    public void ExplainVerySpecial()
    {
        tmp = (int)((ohudaTop - Input.mousePosition.y) / ohudaSpace);
        if ((specialLv - 1) / 5 <= tmp && specialLv != 25)
        {
            explain.text = explains[3];
        }
        else if (tmp % 3 == 0)
        {
            explain.text = explains[12];
        }
        else if (tmp == 1)
        {
            explain.text = explains[13];
        }
        else if (tmp == 2)
        {
            explain.text = explains[14];
        }
        else
        {
            explain.text = explains[15];
        }
    }

    public void ExplainButton()
    {
        tmp = (int)((Input.mousePosition.x - buttonLeft) / buttonSpace);
        explain.text = explains[tmp];
    }

    IEnumerator IncreaseFullAmount()
    {
        pConv.SetActive(true);
        endAmount = ohuda.fillAmount + 0.2f;
        yield return GameSystem.Instance.HalfSecond;
        pDiver.SetActive(true);
        while (ohuda.fillAmount < endAmount)
        {
            ohuda.fillAmount += 0.01f;
            yield return w;
        }
        ohuda.fillAmount = endAmount;
        pConv.SetActive(false);
        pDiver.SetActive(false);
        if (endAmount >= 1)
        {
            ohuda.CrossFadeColor(Color.black, 0.5f, true, false);
            yield return GameSystem.Instance.HalfSecond;
            ohuda.CrossFadeColor(Color.white, 0.5f, true, false);
            yield return GameSystem.Instance.HalfSecond;
        }
        isRunning = false;
        //buttons[0].interactable = true;
    }
}
