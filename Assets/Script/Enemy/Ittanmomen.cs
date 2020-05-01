using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ittanmomen : Enemy
{
    static readonly Vector2 InitialVeclocity = new Vector2(7f, 3f);

    //[SerializeField] CircleCollider2D cc = default;
    [SerializeField] CircleCollider2D hitArea = default;
    [SerializeField] Sprite stunS = default;

    Vector2 vibrationCenter;
    Vector2 tmp;

    // Start is called before the first frame update
    public override void Set()
    {
        if (!resetFlag)
            return;

        base.Set();
        vibrationCenter = transform.position;
        rb.gravityScale = 0;
        animator.enabled = true;
        act = FloatingProcess;
    }

    public override void Act()
    {
        MouseEvent();
        act();
        hitArea.transform.position = transform.position;
        SetActiveAreaPosition();
    }

    void MouseEvent()
    {
        if (stun || invincible || !inRange || !Input.GetMouseButtonDown(0) || !hitArea.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            return;

        if (GameSystem.Instance.Stop)
            return;

        StartCoroutine(CoroutineNameDamageEffect);
        Damage(Mikochan.Instance.KamiAttack);
        rb.velocity = Vector2.zero;
        if (hp <= 0)
        {
            rb.gravityScale = 1.0f;
            stun = true;
            animator.enabled = false;
            sr.sprite = stunS;
            act = StunProcess;
            ActiveStunEffect(true);
            counter = 0;
        }
    }
    

    public void FloatingProcess()
    {
        rb.AddForce(Vector2.up * (vibrationCenter.y - transform.position.y) * 1.2f);
        if (gameObject.transform.position.y >= vibrationCenter.y && rb.velocity.y >= 0)
        {
            rb.velocity = Vector2.zero;
            vibrationCenter = transform.position;
            act = DriftingProcess;
            rb.AddForce(InitialVeclocity, ForceMode2D.Impulse);
        }
    }

    public void DriftingProcess()
    {
        tmp = vibrationCenter - (Vector2)transform.position;
        tmp.y *= 20f;
        rb.AddForce(tmp);
    }

    void StunProcess()
    {
        if (counter++ < 300 || isSealed)
            return;

        stun = false;
        act = FloatingProcess;
        rb.gravityScale = 0;
        animator.enabled = true;
        ActiveStunEffect(false);
        Revival();
        counter = 0;
    }

    //void Move()
    //{
    //    if (descent)
    //    {
    //        rb.AddForce(Vector2.up * (vibrationCenter.y - transform.position.y));
    //        if (rb.position.y > vibrationCenter.y && rb.velocity.y > 0)
    //        {
    //            descent = false;
    //            rb.velocity = Vector2.zero;
    //            vibrationCenter = (Vector2)(transform.position) + (Vector2.right * MoveRange);
    //        }
    //    }
    //    else
    //    {
    //        rb.AddForce(Vector2.right * (vibrationCenter.x - transform.position.x));
    //    }
    //    hitArea.transform.position = transform.position;
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (act == DriftingProcess && collision.gameObject.CompareTag("mikochan"))
        {
            rb.velocity = Vector2.zero;
            rb.AddForce((collision.gameObject.transform.position - transform.position) * 1.2f, ForceMode2D.Impulse);
            act = FloatingProcess;
            return;
        }

        if (stun && collision.gameObject.CompareTag("Ground"))
        {
            rb.velocity = Vector2.zero;
        }

    }

    private IEnumerator DamageEffectCoroutine()
    {
        DamageEffect();
        yield return null;
        yield return null;
        yield return null;
        ResetMaterial();
    }
}
