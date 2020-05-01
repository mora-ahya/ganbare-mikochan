using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitotsumekozou : Enemy
{
    static readonly float VelocityX = 6.0f;
    static readonly float StumbleDis = 2f;
    static readonly float StumbleForce = 10.0f;
    static readonly float DifStandLie = 0.03f;
    static readonly string AnimBoolIsRun = "isRun";
    static readonly string AnimBoolIsStumble = "isStumble";

    [SerializeField] CapsuleCollider2D cc = default;
    [SerializeField] CapsuleCollider2D hitArea = default;
    [SerializeField] Sprite stunSprite = default;

    Vector3 tmp;
    int dir = -1;

    public override void Set()
    {
        if (gameObject.activeSelf)
        {
            animator.SetBool(AnimBoolIsRun, false);
            animator.SetBool(AnimBoolIsStumble, false);
        }

        ChangeTriggerDir(false);
        counter = 0;
        act = SearchingProcess;

        if (!resetFlag)
            return;
        
        animator.enabled = true;
        base.Set();
    }

    public override void Act()
    {
        MouseEvent();
        act();
        hitArea.transform.position = transform.position;
        SetActiveAreaPosition();
    }

    void SearchingProcess()
    {
        if (counter++ != 90)
            return;

        counter = 0;
        dir *= -1;
        tmp = gameObject.transform.localScale;
        tmp.x *= -1;
        gameObject.transform.localScale = tmp;
    }

    void RunningProcess()
    {
        if (Mathf.Abs(Mikochan.Instance.transform.position.x - transform.position.x) < StumbleDis)
        {
            act = StumblingProcess;
            rb.velocity = Vector2.zero;
            tmp = Vector2.right * dir * StumbleForce;
            rb.AddForce(tmp, ForceMode2D.Impulse);
            animator.SetBool(AnimBoolIsStumble, true);
            animator.SetBool(AnimBoolIsRun, false);
            ChangeTriggerDir(true);
            return;
        }

        if (Mathf.Abs(rb.velocity.x) < VelocityX)
        {
            rb.AddForce(Vector2.right * VelocityX * dir);
        }
    }

    void StumblingProcess()
    {
        if (rb.velocity.x * dir < 0.1f)
        {
            rb.velocity = Vector2.zero;
            act = LyingProcess;
            counter = 0;
            return;
        }

        rb.AddForce(transform.right * -dir * 6);
    }

    void LyingProcess()
    {
        counter++;
        rb.velocity = Vector3.zero;
        if (hp <= 0)
        {
            stun = true;
            ActiveStunEffect(true);
            sr.sprite = stunSprite;
            animator.enabled = false;
            act = StunProcess;
            counter = 0;
            return;
        }

        if (counter == 20)
        {
            animator.SetBool(AnimBoolIsStumble, false);
            ChangeTriggerDir(false);
            return;
        }

        if (counter == 40)
        {
            counter = 0;
            act = SearchingProcess;
            return;
        }
    }

    void StunProcess()
    {
        if (counter++ < 300 || isSealed)
            return;

        stun = false;
        counter = 0;
        act = SearchingProcess;
        animator.enabled = true;
        animator.SetBool(AnimBoolIsRun, false);
        animator.SetBool(AnimBoolIsStumble, false);
        ChangeTriggerDir(false);
        ActiveStunEffect(false);
        Revival();
    }

    void MouseEvent()
    {
        if (stun || invincible || !inRange || !Input.GetMouseButtonDown(0) || !hitArea.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            return;

        if (GameSystem.Instance.Stop)
            return;

        StartCoroutine(CoroutineNameDamageEffect);
        Damage(Mikochan.Instance.KamiAttack);
        if (hp <= 0 && act != StumblingProcess)
        {
            ChangeTriggerDir(true);
            act = StunProcess;
            counter = 0;
            rb.velocity = Vector3.zero;
            stun = true;
            ActiveStunEffect(true);
            sr.sprite = stunSprite;
            animator.enabled = false;
        }
    }

    void ChangeTriggerDir(bool b)
    {
        if (b)
        {
            if (cc.direction != CapsuleDirection2D.Horizontal) {
                cc.direction = CapsuleDirection2D.Horizontal;
                cc.offset += Vector2.down * DifStandLie;
                tmp.Set(cc.size.y, cc.size.x, 0);
                cc.size = tmp;
                hitArea.direction = CapsuleDirection2D.Horizontal;
                hitArea.offset += Vector2.down * DifStandLie;
                tmp.Set(hitArea.size.y, hitArea.size.x, 0);
                hitArea.size = tmp;
            }
            return;
        }

        if (cc.direction != CapsuleDirection2D.Vertical)
        {
            cc.direction = CapsuleDirection2D.Vertical;
            cc.offset += Vector2.up * DifStandLie;
            tmp.Set(cc.size.y, cc.size.x, 0);
            cc.size = tmp;
            hitArea.direction = CapsuleDirection2D.Vertical;
            hitArea.offset += Vector2.up * DifStandLie;
            tmp.Set(hitArea.size.y, hitArea.size.x, 0);
            hitArea.size = tmp;

        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (act == SearchingProcess && collision.gameObject.CompareTag("mikochan"))
        {
            act = RunningProcess;
            animator.SetBool(AnimBoolIsRun, true);
        }
    }

    IEnumerator DamageEffectCoroutine()
    {
        DamageEffect();
        yield return null;
        yield return null;
        yield return null;
        ResetMaterial();
    }
}
