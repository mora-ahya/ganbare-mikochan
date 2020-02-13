using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gashadokuro : Enemy
{
    private readonly int SHOT_COUNT = 4;

    [SerializeField] GashaShot[] gashaShots;
    [SerializeField] GameObject miko;
    [SerializeField] BoxCollider2D handR;
    [SerializeField] BoxCollider2D handL;
    [SerializeField] Animator animator;
    [SerializeField] HowlManager hm;
    [SerializeField] Transform mouseTrans;
    [SerializeField] CameraManager cm;
    [SerializeField] FirstBossEvent fbe;
    private float defaultPosX;
    private WaitForSeconds wait = new WaitForSeconds(2f);//ダメージごとに短く
    private readonly WaitForSeconds wait2 = new WaitForSeconds(3f);
    private AnimatorStateInfo anim;
    private int swingCount = 0;
    private int shotInterval = 2;
    private float amountX = 0;
    private float shotV = 1.0f;
    private int shotHP = 3;
    private readonly float speed = 0.2f;
    private readonly float handBoundary = 4.0f; //がしゃが降り下ろしの1と2のどちらを使うかその境界値
    private readonly float firstHandDis = 5.5f; //振り下ろし1の手の位置とがしゃ本体の位置の距離
    private readonly float secondHandDis = 2.5f; //振り下ろし2の手の位置とがしゃ本体の位置の距離
    private float d;
    private bool move = false;
    private bool runCoroutine = false;
    State state = State.Appearance;

    enum State
    {
        Appearance,
        Sprinkle,
        Swing,
        Return,
        RowAway,
        Stun,
        Recovery,
        Beaten
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultPosX = transform.position.x;
        HandCollider(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Appearance: //登場シーン
                if (!runCoroutine)
                {
                    if (!cm.GetShake)
                    {
                        cm.Shake(true);
                    }
                    anim = animator.GetCurrentAnimatorStateInfo(0);
                    if (anim.IsName("cooltime"))
                    {
                        HandCollider(true);
                        runCoroutine = true;
                        cm.Shake(false);
                        StartCoroutine("Cooltime");
                    }
                    else if (anim.IsName("laugh") && !hm.gameObject.activeSelf)
                    {
                        if (!cm.GetShake)
                        {
                            cm.Shake(true);
                        }
                        hm.Howl(mouseTrans.position, 0.5f);
                    }
                    else if (cm.GetShake && anim.IsName("appearance") && anim.normalizedTime > 1)
                    {
                        cm.Shake(false);
                    }
                }
                break;

            case State.Sprinkle: //腕の振り上げ
                Sprinkle();
                state = State.Swing;
                break;

            case State.Swing: //腕の振り下げs
                if (!runCoroutine)
                {
                    anim = animator.GetCurrentAnimatorStateInfo(0);
                    if (anim.IsName("cooltime"))
                    {
                        swingCount++;
                        if (!CheckShots() && swingCount == shotInterval)
                        {
                            animator.SetBool("OpenMouse", true);
                        }
                        ResetHandBool();
                        ResetSwingBool();
                        runCoroutine = true;
                        StartCoroutine("Cooltime");
                    }
                }
                break;

            case State.Return: //ダメージを与えられ真ん中に戻る
                anim = animator.GetCurrentAnimatorStateInfo(0);
                if (anim.IsName("hottime"))
                {
                    if (hp > 0)
                    {
                        ReturnDefaultPos();
                        state = State.RowAway;
                        swingCount = 0;
                    }
                    else
                    {

                    }
                }
                break;

            case State.RowAway: //薙ぎ払い
                if (!move && !runCoroutine)
                {
                    runCoroutine = true;
                    StartCoroutine("Hottime");
                }
                break;

            case State.Stun: //気絶状態
                if (handL.enabled)
                {
                    anim = animator.GetCurrentAnimatorStateInfo(0);
                    if (anim.IsName("broken"))
                    {
                        HandCollider(false);
                    }
                }
                else if (swingCount == SHOT_COUNT)
                {
                    state = State.Recovery;
                }
                else if (!runCoroutine)
                {
                    runCoroutine = true;
                    StartCoroutine("ShotInterval");
                }
                break;

            case State.Recovery: //気絶から復帰
                if (!runCoroutine)
                {
                    if (animator.GetBool("Recovery"))
                    {
                        anim = animator.GetCurrentAnimatorStateInfo(0);
                        if (anim.IsName("cooltime"))
                        {
                            ResetHandBool();
                            ResetSwingBool();
                            Upgrade();
                            HandCollider(true);
                            swingCount = 0;
                            state = State.Sprinkle;
                        }
                    }
                    else
                    {
                        if (!CheckShots())
                        {
                            animator.SetBool("Recovery", true);
                        }
                    }
                }
                break;
        }
        Move();
        MakeShot();
    }

    void Sprinkle() //腕の振り上げ
    {
        float tmp = miko.transform.position.x - transform.position.x;
        if (tmp > 0)
        {
            animator.SetBool("LHand", true);
            d = 1;
            //左手
        }
        else
        {
            animator.SetBool("RHand", true);
            d = -1;
            //右手
        }
        tmp = Mathf.Abs(tmp);
        amountX = tmp - handBoundary;
        if (amountX < 0)
        {
            animator.SetBool("useSecond", true);
            amountX = tmp - secondHandDis;
            //振り下ろし2を使用し、sHDとmikoの距離を測る
        }
        else
        {
            animator.SetBool("useFirst", true);
            amountX = tmp - firstHandDis;
            //振り下ろし1を使用し、fHDとmikoの距離を測る
        }
        tmp = Mathf.Abs(amountX);
        if (tmp != 0) {
            d *= amountX / tmp;
        }
        amountX = tmp;
        move = true;
    }

    void Move()
    {
        if (move)
        {
            if (amountX >= speed)
            {
                transform.position += new Vector3(d * speed, 0, 0);
                amountX -= speed;
            }
            else
            {
                transform.position += new Vector3(d * amountX, 0, 0);
                move = false;
            }
        }
    }

    void ResetHandBool() //振り上げアニメーションのリセット
    {
        if (animator.GetBool("LHand"))
        {
            animator.SetBool("LHand", false);
        }
        else if (animator.GetBool("RHand"))
        {
            animator.SetBool("RHand", false);
        }
    }

    void ResetSwingBool() //振り下げアニメーションのリセット
    {
        if (animator.GetBool("useFirst"))
        {
            animator.SetBool("useFirst", false);
        }
        else if (animator.GetBool("useSecond"))
        {
            animator.SetBool("useSecond", false);
        }
    }

    void HandCollider(bool b) //手の当たり判定の有効化,無効化
    {
        handR.enabled = b;
        handL.enabled = b;
    }

    void ReturnDefaultPos() //中心へ戻る動き
    {
        move = true;
        amountX = defaultPosX - transform.position.x;
        if (amountX != 0)
        {
            d = amountX / Mathf.Abs(amountX);
        }
        amountX = Mathf.Abs(amountX);
    }

    void MakeShot() //がしゃショットの有効化
    {
        if (animator.GetBool("OpenMouse"))
        {
            anim = animator.GetCurrentAnimatorStateInfo(1);
            if (anim.IsName("Open") && anim.normalizedTime >= 1)
            {
                foreach (GashaShot gashaShot in gashaShots)
                {
                    if (!gashaShot.gameObject.activeSelf)
                    {
                        gashaShot.gameObject.SetActive(true);
                        gashaShot.Set(shotV, shotHP);
                        animator.SetBool("OpenMouse", false);
                        break;
                    }
                }
            }
        }
    }

    bool CheckShots() //ショットがまだ残っているかどうか
    {
        foreach (GashaShot gashaShot in gashaShots)
        {
            if (gashaShot.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    void Upgrade() //一定ダメージごとに攻撃を早くする
    {
        if (hp <= 1)
        {
            wait = new WaitForSeconds(0.1f);
            animator.SetFloat("Multiplier", 2.0f);
            shotInterval = 8;
            shotV = 3.0f;
            shotHP = 10;
        }
        else if (hp <= 6)
        {
            wait = new WaitForSeconds(0.3f);
            animator.SetFloat("Multiplier", 1.5f);
            shotInterval = 6;
            shotV = 2.0f;
        } else if (hp <= 11)
        {
            wait = new WaitForSeconds(1.0f);
            animator.SetFloat("Multiplier", 1.0f);
            shotInterval = 4;
        }
    }

    private IEnumerator Cooltime() //腕を振り下ろした後の待機時間
    {
        yield return wait;
        if (animator.GetBool("Damaged"))
        {
            state = State.Return;
        }
        else if (animator.GetBool("Beaten"))
        {
            state = State.Beaten;
        }
        else
        {
            state = State.Sprinkle;
        }
        runCoroutine = false;
    }

    private IEnumerator Hottime() //ダメージを食らって薙ぎ払うまでの待機時間
    {
        yield return wait2;
        animator.SetBool("Damaged", false);
        animator.SetBool("Recovery", false);
        state = State.Stun;
        runCoroutine = false;
    }

    private IEnumerator DamageEffects() //ダメージを受けたとき赤くなる
    {
        DamageEffect();
        yield return null;
        ResetM();
    }

    private IEnumerator ShotInterval() //ショットの間隔
    {
        yield return wait2;
        animator.SetBool("OpenMouse", true);
        swingCount++;
        runCoroutine = false;
    }

    public void Damage() //ダメージを受けたときの処理
    {
        Damage(1);
        StartCoroutine("DamageEffects");
        if (state != State.Stun && state != State.Recovery)
        {
            if (hp <= 0)
            {
                animator.SetBool("Beaten", true);
                HandCollider(false);
                fbe.BeatenBoss();
            }
            else
            {
                animator.SetBool("Damaged", true);
            }
        }
    }
}
