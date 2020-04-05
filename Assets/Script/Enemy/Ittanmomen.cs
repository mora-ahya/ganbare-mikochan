using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ittanmomen : Enemy
{
    static readonly Vector2 InitialVeclocity = new Vector2(7f, 3f);

    [SerializeField] Animator _animator = default;
    //[SerializeField] CircleCollider2D cc = default;
    [SerializeField] CircleCollider2D hitArea = default;
    [SerializeField] Sprite stunS = default;

    Vector2 vibrationCenter;
    Vector2 tmp;
    Mikochan miko;
    Action act;
    // Start is called before the first frame update
    void Start()
    {
        vibrationCenter = transform.position;
        miko = Mikochan.Instance;
    }

    public override void Set()
    {
        inRange = false;
        rb.gravityScale = 0;
        _animator.enabled = true;
        Revival();
        ResetMaterial();
        ActiveStunEffect(false);
        act = FloatingProcess;
        stun = false;

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

    void MouseEvent()
    {
        if (!invincible && inRange && Input.GetMouseButtonDown(0))
        {
            //Debug.Log(self.InRange);
            if (hitArea.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                if (!GameSystem.Instance.Stop)
                {
                    StartCoroutine(CoroutineNameDamageEffect);
                    Damage(miko.KamiAttack);
                    rb.velocity = Vector2.zero;
                    if (hp <= 0)
                    {
                        rb.gravityScale = 1.0f;
                        stun = true;
                        _animator.enabled = false;
                        sr.sprite = stunS;
                        act = null;
                        ActiveStunEffect(true);
                        StartCoroutine(CoroutineNameRevival);
                    }
                }
            }
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

    private IEnumerator RevivalCoroutine()
    {
        yield return GameSystem.Instance.FiveSecond;
        stun = false;
        act = FloatingProcess;
        rb.gravityScale = 0;
        _animator.enabled = true;
        ActiveStunEffect(false);
        Revival();
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
