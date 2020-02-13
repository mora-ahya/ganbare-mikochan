using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Karakasa : Enemy
{
    static readonly float JUMP_POWER = 5.0f;
    static readonly float JUMP_POWER2 = 15.0f;
    static readonly float JUMP_POWER_X = 1.0f;
    static readonly float FLOATING_POWER = 10.0f;
    static readonly float BOXCOLLIDER_OFFSETY = -0.195f;
    static readonly Vector2 COLLIDER_OFFSET = new Vector2(-0.02f, -0.04f);

    [SerializeField] CapsuleCollider2D cc;
    [SerializeField] CapsuleCollider2D hitArea;
    [SerializeField] BoxCollider2D bc;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Sprite[] s;
    readonly WaitForSeconds stunTime = new WaitForSeconds(5.0f);
    readonly WaitForSeconds squatTime = new WaitForSeconds(0.5f);

    private Vector2 jumpPower = new Vector2();
    private bool damaged;
    private int count = 1;
    private State state = State.Squat;
    Vector3 tmp = new Vector3();
    Mikochan miko;

    enum State
    {
        Jumping,
        Squat,
        Glide,
        Stun,
        Awake
    }

    void Start()
    {
        miko = Mikochan.Instance;
    }

    // Start is called before the first frame update
    public override void Set()
    {
        stun = false;
        inRange = false;
        Revival();
        sr.sprite = s[(int)State.Squat];
        state = State.Squat;
        count = 1;
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
        //Debug.Log(isGround);
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
                    damaged = true;
                    StartCoroutine("DamageEffects");
                    Damage(miko.KamiAttack);
                    rb.velocity = Vector2.zero;
                    Debug.Log(HP);
                    if (hp <= 0)
                    {
                        stun = true;
                        sr.sprite = s[(int)State.Stun];
                        ActiveStunEffect(true);
                        StartCoroutine("Revivals");
                    }
                }
            }
        }
    }

    void Jump()
    {
        if (miko.transform.position.x < transform.position.x)
        {
            jumpPower.x = -JUMP_POWER_X;
            if (sr.flipX)
            {
                sr.flipX = false;
                ColliderFlip(false);
            }
        }
        else
        {
            jumpPower.x = JUMP_POWER_X;
            if (!sr.flipX)
            {
                sr.flipX = true;
                ColliderFlip(true);
            }
        }
        sr.sprite = s[(int)State.Jumping];
        state = State.Jumping;
        jumpPower.y = count == 2 ? JUMP_POWER2 : JUMP_POWER;
        rb.AddForce(jumpPower, ForceMode2D.Impulse);
        count = (count + 1) % 3;
    }

    void Move()
    {
        if (!damaged && rb.velocity.y < 0 && count == 0)
        {
            if (state != State.Glide)
            {
                state = State.Glide;
                sr.sprite = s[(int)State.Glide];
            }
            rb.AddForce(Vector2.down * FLOATING_POWER * rb.velocity.y);
            if (miko.transform.position.x < transform.position.x)
            {
                rb.AddForce((rb.velocity.x < 0 ? JUMP_POWER2 / 5 : JUMP_POWER2) * Vector2.left);
                if (sr.flipX)
                {
                    sr.flipX = false;
                    ColliderFlip(false);
                }
            }
            else
            {
                rb.AddForce((rb.velocity.x > 0 ? JUMP_POWER2 / 5 : JUMP_POWER2) * Vector2.right);
                if (!sr.flipX)
                {
                    sr.flipX = true;
                    ColliderFlip(true);
                }
            }
        }
        hitArea.transform.position = transform.position;
    }

    void ColliderFlip(bool b)
    {
        if (b)
        {
            tmp.Set(-COLLIDER_OFFSET.x, COLLIDER_OFFSET.y, 0);
            cc.offset = tmp;
            hitArea.offset = tmp;
            tmp.y = BOXCOLLIDER_OFFSETY;
            bc.offset = tmp;
        }
        else
        {
            tmp.Set(COLLIDER_OFFSET.x, COLLIDER_OFFSET.y, 0);
            cc.offset = tmp;
            hitArea.offset = tmp;
            tmp.y = BOXCOLLIDER_OFFSETY;
            bc.offset = tmp;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(true);
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!stun)
            {
                rb.velocity = Vector2.zero;
                sr.sprite = s[(int)State.Squat];
                state = State.Squat;
                StartCoroutine("Squat");
                if (damaged)
                {
                    damaged = false;
                }
            }
        }
    }

    private IEnumerator Revivals()
    {
        yield return stunTime;
        ActiveStunEffect(false);
        stun = false;
        Revival();
        sr.sprite = s[(int)State.Squat];
        state = State.Squat;
        StartCoroutine("Squat");
        count = 0;
    }

    private IEnumerator DamageEffects()
    {
        DamageEffect();
        yield return null;
        yield return null;
        yield return null;
        ResetM();
    }

    private IEnumerator Squat()
    {
        yield return squatTime;
        if (!stun)
        {
            state = State.Jumping;
            sr.sprite = s[(int)State.Jumping];
            if (damaged)
            {
                damaged = false;
            }
            Jump();
        }
    }
}
