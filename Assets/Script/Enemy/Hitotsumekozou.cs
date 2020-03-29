using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitotsumekozou : Enemy
{
    static readonly float VELOCITY_X = 6.0f;
    static readonly float STUMBLE_DIS = 2f;
    static readonly float STUMBLE_FORCE = 10.0f;
    static readonly float DIF_STAND_LIE = 0.03f;

    [SerializeField] Animator _animator;
    [SerializeField] CapsuleCollider2D cc;
    [SerializeField] CapsuleCollider2D hitArea;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Sprite stunSprite;
    readonly WaitForSeconds stunTime = new WaitForSeconds(5.0f);
    readonly WaitForSeconds coolTime = new WaitForSeconds(0.5f);

    private GameObject target = null;
    private Vector3 tmp;
    private bool stumble = false;
    private bool search = true;
    private int dir = 1;
    private IEnumerator routine;
    private Mikochan miko;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (routine != null)
        {
            StartCoroutine(routine);
        }
    }

    void Start()
    {
        miko = Mikochan.Instance;
    }

    public override void Set()
    {
        stun = false;
        inRange = false;
        search = true;
        target = null;
        stumble = false;
        _animator.enabled = true;
        if (gameObject.activeSelf)
        {
            _animator.SetBool("isRun", false);
            _animator.SetBool("isStumble", false);
        }
        ChangeTriggerDir(false);
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
                if (!GameSystem.stop && hp > 0)
                {
                    StartCoroutine("DamageEffects");
                    Damage(miko.KamiAttack);
                    Debug.Log(hp);
                    if (!stun && hp <= 0 && !stumble)
                    {
                        if (cc.direction != CapsuleDirection2D.Horizontal)
                        {
                            ChangeTriggerDir(true);
                        }
                        target = null;
                        rb.velocity = Vector3.zero;
                        stun = true;
                        ActiveStunEffect(true);
                        sr.sprite = stunSprite;
                        _animator.enabled = false;
                        StartCoroutine("Revivals");
                    }
                }
            }
        }
    }

    void Move()
    {
        if (target != null)
        {
            if (Mathf.Abs(target.transform.position.x - transform.position.x) < STUMBLE_DIS)
            {
                target = null;
                stumble = true;
                rb.velocity = Vector2.zero;
                tmp = Vector2.right * dir * STUMBLE_FORCE;
                rb.AddForce(tmp, ForceMode2D.Impulse);
                _animator.SetBool("isStumble", true);
                _animator.SetBool("isRun", false);
                ChangeTriggerDir(true);
            }
            else if (Mathf.Abs(rb.velocity.x) < VELOCITY_X)
            {
                rb.AddForce(Vector2.right * VELOCITY_X * dir);
            }
        }
        else if (stumble)
        {
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                rb.velocity = Vector2.zero;
                stumble = false;
                if (hp < 1)
                {
                    target = null;
                    rb.velocity = Vector3.zero;
                    stun = true;
                    ActiveStunEffect(true);
                    sr.sprite = stunSprite;
                    _animator.enabled = false;
                    StartCoroutine("Revivals");
                }
                else
                {
                    routine = Stumble();
                    StartCoroutine(routine);
                }
            }
            else
            {
                rb.AddForce(transform.right * -dir * 6);
            }
        }
        hitArea.transform.position = transform.position;
    }

    void ChangeTriggerDir(bool b)
    {
        if (b)
        {
            if (cc.direction != CapsuleDirection2D.Horizontal) {
                cc.direction = CapsuleDirection2D.Horizontal;
                cc.offset += Vector2.down * DIF_STAND_LIE;
                tmp.Set(cc.size.y, cc.size.x, 0);
                cc.size = tmp;
                hitArea.direction = CapsuleDirection2D.Horizontal;
                hitArea.offset += Vector2.down * DIF_STAND_LIE;
                tmp.Set(hitArea.size.y, hitArea.size.x, 0);
                hitArea.size = tmp;
            }
        }
        else
        {
            if (cc.direction != CapsuleDirection2D.Vertical)
            {
                cc.direction = CapsuleDirection2D.Vertical;
                cc.offset += Vector2.up * DIF_STAND_LIE;
                tmp.Set(cc.size.y, cc.size.x, 0);
                cc.size = tmp;
                hitArea.direction = CapsuleDirection2D.Vertical;
                hitArea.offset += Vector2.up * DIF_STAND_LIE;
                tmp.Set(hitArea.size.y, hitArea.size.x, 0);
                hitArea.size = tmp;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (hp > 0 && search && collision.gameObject.CompareTag("mikochan"))
        {
            target = collision.gameObject;
            search = false;
            sr.flipX = target.transform.position.x > transform.position.x;
            dir = sr.flipX ? 1 : -1;
            tmp.Set(Mathf.Abs(cc.offset.x) * dir, cc.offset.y, 0);
            cc.offset = tmp;
            hitArea.offset = tmp;
            _animator.SetBool("isRun", true);
        }
    }

    private IEnumerator Revivals()
    {
        yield return stunTime;
        stun = false;
        search = true;
        _animator.enabled = true;
        _animator.SetBool("isRun", false);
        _animator.SetBool("isStumble", false);
        ChangeTriggerDir(false);
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

    private IEnumerator Stumble()
    {
        yield return coolTime;
        if (hp > 0)
        {
            _animator.SetBool("isStumble", false);
            ChangeTriggerDir(false);
            yield return coolTime;
            search = true;
        }
        routine = null;
    }
}
