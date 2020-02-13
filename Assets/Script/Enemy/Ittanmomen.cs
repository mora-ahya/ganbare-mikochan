using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ittanmomen : Enemy
{
    static readonly float MOVE_RANGE = 7.5f;

    [SerializeField] Animator _animator;
    [SerializeField] CircleCollider2D cc;
    [SerializeField] CircleCollider2D hitArea;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Sprite stunS;
    readonly WaitForSeconds stunTime = new WaitForSeconds(5.0f);

    private Vector2 defaultPoss;
    private bool descent = false;
    private Mikochan miko;
    // Start is called before the first frame update
    void Start()
    {
        defaultPoss = (Vector2)(transform.position) + (Vector2.right * MOVE_RANGE);
        miko = Mikochan.Instance;
    }

    public override void Set()
    {
        stun = false;
        inRange = false;
        rb.gravityScale = 0;
        descent = false;
        defaultPoss = (Vector2)(transform.position) + (Vector2.right * MOVE_RANGE);
        _animator.enabled = true;
        Revival();
        ResetM();
        ActiveStunEffect(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (resetFlag)
        {
            Set();
            resetFlag = false;
        }
        if (!stun)
        {
            MouseEvent();
            Move();
        }
        SetActiveAreaPosition();
    }

    void MouseEvent()
    {
        if (!invincible && inRange && Input.GetMouseButtonDown(0))
        {
            //Debug.Log(self.InRange);
            if (hitArea.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                if (!GameSystem.stop)
                {
                    StartCoroutine("DamageEffects");
                    Damage(miko.KamiAttack);
                    rb.velocity = Vector2.zero;
                    Debug.Log(hp);
                    if (hp <= 0)
                    {
                        rb.gravityScale = 1.0f;
                        stun = true;
                        _animator.enabled = false;
                        sr.sprite = stunS;
                        ActiveStunEffect(true);
                        StartCoroutine("Revivals");
                    }
                }
            }
        }
    }

    void Move()
    {
        if (descent)
        {
            rb.AddForce(Vector2.up * (defaultPoss.y - transform.position.y));
            if (rb.position.y > defaultPoss.y && rb.velocity.y > 0)
            {
                descent = false;
                rb.velocity = Vector2.zero;
                defaultPoss = (Vector2)(transform.position) + (Vector2.right * MOVE_RANGE);
            }
        }
        else
        {
            rb.AddForce(Vector2.right * (defaultPoss.x - transform.position.x));
        }
        hitArea.transform.position = transform.position;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!descent)
        {
            if (collision.gameObject.CompareTag("mikochan"))
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(collision.gameObject.transform.position - transform.position, ForceMode2D.Impulse);
                descent = true;
            }
        }
    }

    private IEnumerator Revivals()
    {
        yield return stunTime;
        stun = false;
        descent = true;
        rb.gravityScale = 0;
        _animator.enabled = true;
        ActiveStunEffect(false);
        Revival();
    }

    private IEnumerator DamageEffects()
    {
        DamageEffect();
        yield return null;
        yield return null;
        yield return null;
        ResetM();
    }
}
