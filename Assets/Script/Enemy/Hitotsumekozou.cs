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

    [SerializeField] Animator animator = default;
    [SerializeField] CapsuleCollider2D cc = default;
    [SerializeField] CapsuleCollider2D hitArea = default;
    [SerializeField] Sprite stunSprite = default;

    Vector3 tmp;
    int dir = -1;
    int counter = 0;
    Action act = null;

    public override void Set()
    {
        stun = false;
        inRange = false;
        animator.enabled = true;
        act = SearchingProcess;
        counter = 0;
        if (gameObject.activeSelf)
        {
            animator.SetBool(AnimBoolIsRun, false);
            animator.SetBool(AnimBoolIsStumble, false);
        }
        ChangeTriggerDir(false);
        Revival();
        ResetMaterial();
        ActiveStunEffect(false);
    } 

    public override void Act()
    {
        if (resetFlag)
        {
            Set();
            resetFlag = false;
        }
        if (!stun)
        {
            MouseEvent();
            act?.Invoke();
        }
        hitArea.transform.position = transform.position;
        SetActiveAreaPosition();
    }

    void SearchingProcess()
    {
        if (counter == 90)
        {
            counter = 0;
            dir *= -1;
            tmp = gameObject.transform.localScale;
            tmp.x *= -1;
            gameObject.transform.localScale = tmp;
            return;
        }
        counter++;
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
        if (Mathf.Abs(rb.velocity.x) < 0.1f)
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
        if (hp <= 0)
        {
            rb.velocity = Vector3.zero;
            stun = true;
            ActiveStunEffect(true);
            sr.sprite = stunSprite;
            animator.enabled = false;
            act = null;
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

    void MouseEvent()
    {
        if (!invincible && inRange && Input.GetMouseButtonDown(0))
        {
            if (hitArea.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                if (!GameSystem.Instance.Stop)
                {
                    StartCoroutine(CoroutineNameDamageEffect);
                    Damage(Mikochan.Instance.KamiAttack);
                    if (hp <= 0 && act != StumblingProcess)
                    {
                        ChangeTriggerDir(true);
                        act = null;
                        rb.velocity = Vector3.zero;
                        stun = true;
                        ActiveStunEffect(true);
                        sr.sprite = stunSprite;
                        animator.enabled = false;
                        StartCoroutine(CoroutineNameRevival);
                    }
                }
            }
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (act == SearchingProcess && collision.gameObject.CompareTag("mikochan"))
        {
            act = RunningProcess;
            animator.SetBool(AnimBoolIsRun, true);
        }
    }

    IEnumerator RevivalCoroutine()
    {
        yield return GameSystem.Instance.FiveSecond;
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

    IEnumerator DamageEffectCoroutine()
    {
        DamageEffect();
        yield return null;
        yield return null;
        yield return null;
        ResetMaterial();
    }
}
