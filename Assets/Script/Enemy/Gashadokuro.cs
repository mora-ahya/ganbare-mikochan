using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gashadokuro : Enemy
{
    readonly string animNameCooltime = "cooltime";
    readonly string animNameLaugh = "laugh";
    readonly string animNameAppearance = "appearance";
    readonly string animNameHottime = "hottime";
    readonly string animNameBroken = "broken";
    readonly string animNameOpen = "Open";
    readonly string animBoolOpenMouse = "OpenMouse";
    readonly string animBoolRecovery = "Recovery";
    readonly string animBoolLHand = "LHand";
    readonly string animBoolRHand = "RHand";
    readonly string animBoolUseFirst = "useFirst";
    readonly string animBoolUseSecond = "useSecond";
    readonly string animBoolDamaged = "Damaged";
    readonly string animBoolBeaten = "Beaten";
    readonly string animFloatMultiplier = "Multiplier";


    [SerializeField] GashaShot[] gashaShots = default;
    [SerializeField] BoxCollider2D handR = default;
    [SerializeField] BoxCollider2D handL = default;
    [SerializeField] Animator animator = default;
    [SerializeField] Transform mouseTrans = default;
    [SerializeField] CameraManager cm = default;
    [SerializeField] FirstBossEvent fbe = default;
    WaitForSeconds wait = new WaitForSeconds(2f);//ダメージごとに短く
    AnimatorStateInfo anim;
    Action act;
    int swingCount = 0;
    int shotInterval = 2;
    int shotHP = 3;
    readonly int shotCount = 4;
    float amountX = 0;
    float shotV = 1.0f;
    float d;
    float defaultPosX;
    readonly float speed = 0.2f;
    readonly float handBoundary = 4.0f; //がしゃが降り下ろしの1と2のどちらを使うかその境界値
    readonly float firstHandDis = 5.5f; //振り下ろし1の手の位置とがしゃ本体の位置の距離
    readonly float secondHandDis = 2.5f; //振り下ろし2の手の位置とがしゃ本体の位置の距離
    bool move = false;
    bool runCoroutine = false;
    /*
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
    }*/

    // Start is called before the first frame update
    void Start()
    {
        defaultPosX = transform.position.x;
        cm = CameraManager.Instance;
        HandCollider(false);
        wait = GameSystem.Instance.TwoSecond;
        act = AppearanceProcess;
    }

    // Update is called once per frame
    void Update()
    {
        //switch (state)
        //{
        //    case State.Appearance: //登場シーン
        //        if (!runCoroutine)
        //        {
        //            if (!cm.GetShake)
        //            {
        //                cm.Shake(true);
        //            }
        //            anim = animator.GetCurrentAnimatorStateInfo(0);
        //            if (anim.IsName(animNameCooltime))
        //            {
        //                HandCollider(true);
        //                runCoroutine = true;
        //                cm.Shake(false);
        //                StartCoroutine(Cooltime());
        //            }
        //            else if (anim.IsName(animNameLaugh) && !HowlManager.Instance.gameObject.activeSelf)
        //            {
        //                if (!cm.GetShake)
        //                {
        //                    cm.Shake(true);
        //                }
        //                HowlManager.Instance.Howl(mouseTrans.position, 0.5f);
        //            }
        //            else if (cm.GetShake && anim.IsName(animNameAppearance) && anim.normalizedTime > 1)
        //            {
        //                cm.Shake(false);
        //            }
        //        }
        //        break;

        //    case State.Sprinkle: //腕の振り上げ
        //        Sprinkle();
        //        state = State.Swing;
        //        break;

        //    case State.Swing: //腕の振り下げs
        //        if (!runCoroutine)
        //        {
        //            anim = animator.GetCurrentAnimatorStateInfo(0);
        //            if (anim.IsName(animNameCooltime))
        //            {
        //                swingCount++;
        //                if (!CheckShots() && swingCount == shotInterval)
        //                {
        //                    animator.SetBool(animBoolOpenMouse, true);
        //                }
        //                ResetHandBool();
        //                ResetSwingBool();
        //                runCoroutine = true;
        //                StartCoroutine(Cooltime());
        //            }
        //        }
        //        break;

        //    case State.Return: //ダメージを与えられ真ん中に戻る
        //        anim = animator.GetCurrentAnimatorStateInfo(0);
        //        if (anim.IsName(animNameHottime))
        //        {
        //            if (hp > 0)
        //            {
        //                ReturnDefaultPos();
        //                state = State.RowAway;
        //                swingCount = 0;
        //            }
        //            else
        //            {

        //            }
        //        }
        //        break;

        //    case State.RowAway: //薙ぎ払い
        //        if (!move && !runCoroutine)
        //        {
        //            runCoroutine = true;
        //            StartCoroutine(Hottime());
        //        }
        //        break;

        //    case State.Stun: //気絶状態
        //        if (handL.enabled)
        //        {
        //            anim = animator.GetCurrentAnimatorStateInfo(0);
        //            if (anim.IsName(animNameBroken))
        //            {
        //                HandCollider(false);
        //            }
        //        }
        //        else if (swingCount == shotCount)
        //        {
        //            state = State.Recovery;
        //        }
        //        else if (!runCoroutine)
        //        {
        //            runCoroutine = true;
        //            StartCoroutine(ShotInterval());
        //        }
        //        break;

        //    case State.Recovery: //気絶から復帰
        //        if (!runCoroutine)
        //        {
        //            if (animator.GetBool(animBoolRecovery))
        //            {
        //                anim = animator.GetCurrentAnimatorStateInfo(0);
        //                if (anim.IsName(animNameCooltime))
        //                {
        //                    ResetHandBool();
        //                    ResetSwingBool();
        //                    Upgrade();
        //                    HandCollider(true);
        //                    swingCount = 0;
        //                    state = State.Sprinkle;
        //                }
        //            }
        //            else
        //            {
        //                if (!CheckShots())
        //                {
        //                    animator.SetBool(animBoolRecovery, true);
        //                }
        //            }
        //        }
        //        break;
        //}
        Process();
        Move();
        MakeShot();
    }

    void Process()
    {
        act?.Invoke();
    }

    void AppearanceProcess()
    {
        if (runCoroutine)
            return;

        if (!cm.GetShake)
            cm.Shake(true);
        

        anim = animator.GetCurrentAnimatorStateInfo(0);
        if (anim.IsName(animNameCooltime))
        {
            HandCollider(true);
            runCoroutine = true;
            cm.Shake(false);
            StartCoroutine(Cooltime());
            return;
        }

        if (anim.IsName(animNameLaugh) && !HowlManager.Instance.gameObject.activeSelf)
        {
            if (!cm.GetShake)
            {
                cm.Shake(true);
            }
            HowlManager.Instance.Howl(mouseTrans.position, 0.5f);
            return;
        }

        if (cm.GetShake && anim.IsName(animNameAppearance) && anim.normalizedTime > 1)
            cm.Shake(false);
    }

    void SprinkleProcess()
    {
        float tmp = Mikochan.Instance.transform.position.x - transform.position.x;
        if (tmp > 0)
        {
            animator.SetBool(animBoolLHand, true);
            d = 1;
            //左手
        }
        else
        {
            animator.SetBool(animBoolRHand, true);
            d = -1;
            //右手
        }

        tmp = Mathf.Abs(tmp);
        amountX = tmp - handBoundary;
        if (amountX < 0)
        {
            animator.SetBool(animBoolUseSecond, true);
            amountX = tmp - secondHandDis;
            //振り下ろし2を使用し、sHDとmikoの距離を測る
        }
        else
        {
            animator.SetBool(animBoolUseFirst, true);
            amountX = tmp - firstHandDis;
            //振り下ろし1を使用し、fHDとmikoの距離を測る
        }

        tmp = Mathf.Abs(amountX);
        if (tmp != 0)
        {
            d *= amountX / tmp;
        }
        amountX = tmp;
        move = true;
        //state = State.Swing;
        act = SwingProcess;
    }

    void SwingProcess()
    {
        if (runCoroutine)
            return;

        anim = animator.GetCurrentAnimatorStateInfo(0);
        if (!anim.IsName(animNameCooltime))
            return;

        swingCount++;

        if (!CheckShots() && swingCount == shotInterval)
            animator.SetBool(animBoolOpenMouse, true);

        ResetHandBool();
        ResetSwingBool();
        runCoroutine = true;
        StartCoroutine(Cooltime());
    }

    void ReturnProcess()
    {
        anim = animator.GetCurrentAnimatorStateInfo(0);
        if (!anim.IsName(animNameHottime) || hp <= 0)
            return;

        ReturnDefaultPos();
        //state = State.RowAway;
        act = RowAwayProcess;
        swingCount = 0;
    }

    void RowAwayProcess()
    {
        if (move || runCoroutine)
            return;

        runCoroutine = true;
        StartCoroutine(Hottime());
    }

    void StunProcess()
    {
        if (handL.enabled)
        {
            anim = animator.GetCurrentAnimatorStateInfo(0);
            if (anim.IsName(animNameBroken))
                HandCollider(false);
            
            return;
        }

        if (swingCount == shotCount)
        {
            //state = State.Recovery;
            act = RecoveryProcess;
            return;
        }

        if (!runCoroutine)
        {
            runCoroutine = true;
            StartCoroutine(ShotInterval());
        }
    }

    void RecoveryProcess()
    {
        if (runCoroutine)
            return;

        if (animator.GetBool(animBoolRecovery))
        {
            anim = animator.GetCurrentAnimatorStateInfo(0);
            if (anim.IsName(animNameCooltime))
            {
                ResetHandBool();
                ResetSwingBool();
                Upgrade();
                HandCollider(true);
                swingCount = 0;
                //state = State.Sprinkle;
                act = SprinkleProcess;
            }
            return;
        }

        if (!CheckShots())
            animator.SetBool(animBoolRecovery, true);
    }

    void Sprinkle() //腕の振り上げ
    {
        float tmp = Mikochan.Instance.transform.position.x - transform.position.x;
        if (tmp > 0)
        {
            animator.SetBool(animBoolLHand, true);
            d = 1;
            //左手
        }
        else
        {
            animator.SetBool(animBoolRHand, true);
            d = -1;
            //右手
        }
        tmp = Mathf.Abs(tmp);
        amountX = tmp - handBoundary;
        if (amountX < 0)
        {
            animator.SetBool(animBoolUseSecond, true);
            amountX = tmp - secondHandDis;
            //振り下ろし2を使用し、sHDとmikoの距離を測る
        }
        else
        {
            animator.SetBool(animBoolUseFirst, true);
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
        if (!move)
            return;

        if (amountX >= speed)
        {
            transform.position += Vector3.right * d * speed;
            amountX -= speed;
            return;
        }

        transform.position += Vector3.right * d * amountX;
        move = false;
    }

    void ResetHandBool() //振り上げアニメーションのリセット
    {
        if (animator.GetBool(animBoolLHand))
        {
            animator.SetBool(animBoolLHand, false);
            return;
        }

        if (animator.GetBool(animBoolRHand))
        {
            animator.SetBool(animBoolRHand, false);
        }
    }

    void ResetSwingBool() //振り下げアニメーションのリセット
    {
        if (animator.GetBool(animBoolUseFirst))
        {
            animator.SetBool(animBoolUseFirst, false);
            return;
        }

        if (animator.GetBool(animBoolUseSecond))
        {
            animator.SetBool(animBoolUseSecond, false);
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
        if (!animator.GetBool(animBoolOpenMouse))
            return;

        anim = animator.GetCurrentAnimatorStateInfo(1);
        if (!anim.IsName(animNameOpen) || anim.normalizedTime < 1)
            return;

        foreach (GashaShot gashaShot in gashaShots)
        {
            if (!gashaShot.gameObject.activeSelf)
            {
                gashaShot.gameObject.SetActive(true);
                gashaShot.Set(shotV, shotHP);
                animator.SetBool(animBoolOpenMouse, false);
                return;
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
            animator.SetFloat(animFloatMultiplier, 2.0f);
            shotInterval = 8;
            shotV = 3.0f;
            shotHP = 10;
            return;
        }

        if (hp <= 6)
        {
            wait = new WaitForSeconds(0.3f);
            animator.SetFloat(animFloatMultiplier, 1.5f);
            shotInterval = 6;
            shotV = 2.0f;
            return;
        }

        if (hp <= 11)
        {
            wait = GameSystem.Instance.OneSecond;
            animator.SetFloat(animFloatMultiplier, 1.0f);
            shotInterval = 4;
            return;
        }
    }

    private IEnumerator Cooltime() //腕を振り下ろした後の待機時間
    {
        yield return wait;
        runCoroutine = false;
        if (animator.GetBool(animBoolDamaged))
        {
            //state = State.Return;
            act = ReturnProcess;
            yield break;
        }
        if (animator.GetBool(animBoolBeaten))
        {
            //state = State.Beaten;
            act = null;
            yield break;
        }
        //state = State.Sprinkle;
        act = SprinkleProcess;

    }

    private IEnumerator Hottime() //ダメージを食らって薙ぎ払うまでの待機時間
    {
        yield return GameSystem.Instance.ThreeSecond;
        animator.SetBool(animBoolDamaged, false);
        animator.SetBool(animBoolRecovery, false);
        //state = State.Stun;
        act = StunProcess;
        runCoroutine = false;
    }

    private IEnumerator DamageEffects() //ダメージを受けたとき赤くなる
    {
        DamageEffect();
        yield return null;
        yield return null;
        yield return null;
        ResetMaterial();
    }

    private IEnumerator ShotInterval() //ショットの間隔
    {
        yield return GameSystem.Instance.ThreeSecond;
        animator.SetBool(animBoolOpenMouse, true);
        swingCount++;
        runCoroutine = false;
    }

    public void Damage() //ダメージを受けたときの処理
    {
        Damage(1);
        StartCoroutine(DamageEffects());
        if (act == StunProcess || act == RecoveryProcess)
            return;

        if (hp <= 0)
        {
            animator.SetBool(animBoolBeaten, true);
            HandCollider(false);
            fbe.BeatenBoss();
            return;
        }

        animator.SetBool(animBoolDamaged, true);
    }
}
