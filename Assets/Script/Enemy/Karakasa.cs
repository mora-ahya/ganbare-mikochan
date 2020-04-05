using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Karakasa : Enemy
{
    static readonly float JumpPower = 5.0f;
    static readonly float HighJumpPower = 15.0f;
    static readonly float JumpPowerX = 1.0f;
    static readonly float FloatingPower = 10.0f;
    static readonly float BoxColliderOffsetY = -0.195f;
    static readonly Vector2 ColliderOffset = new Vector2(-0.02f, -0.04f);
    static readonly int JumpSprite = 0;
    static readonly int SquatSprite = 1;
    static readonly int GlideSprite = 2;
    static readonly int StunSprite = 3;

    [SerializeField] CapsuleCollider2D cc = default;
    [SerializeField] CapsuleCollider2D hitArea = default;
    [SerializeField] BoxCollider2D bc = default;
    [SerializeField] Sprite[] s = default;

    bool damaged;
    int jumpCount = 1;
    int counter;
    Vector2 tmp = new Vector2();
    Vector2 jumpPower = new Vector2();
    Mikochan miko;
    Action act;

    void Start()
    {
        miko = Mikochan.Instance;
        //act = new Action(SquatProcess);
    }

    // Start is called before the first frame update
    public override void Set()
    {
        if (!resetFlag)
            return;

        stun = false;
        inRange = false;
        Revival();
        sr.sprite = s[SquatSprite];
        act = SquatingProcess;
        counter = 0;
        jumpCount = 1;
        ResetMaterial();
        ActiveStunEffect(false);
        resetFlag = false;
    }
    
    public override void Act()
    {
        if (!stun)
        {
            MouseEvent();
            act?.Invoke();
        }
        hitArea.transform.position = transform.position;
        SetActiveAreaPosition();
        //CurrentMaterialIsDamageEffect();
        //Debug.Log(isGround);
    }

    void SquatingProcess()
    {
        if (counter == 30)
        {
            counter = 0;
            if (damaged)
            {
                damaged = false;
            }

            //Jump
            if (miko.transform.position.x < transform.position.x)
            {
                jumpPower.x = -JumpPowerX;
                if (sr.flipX)
                {
                    sr.flipX = false;
                    ColliderFlip(false);
                }
            }
            else
            {
                jumpPower.x = JumpPowerX;
                if (!sr.flipX)
                {
                    sr.flipX = true;
                    ColliderFlip(true);
                }
            }
            sr.sprite = s[JumpSprite];
            act = JumpingProcess;
            jumpPower.y = jumpCount == 2 ? HighJumpPower : JumpPower;
            rb.AddForce(jumpPower, ForceMode2D.Impulse);
            jumpCount = (jumpCount + 1) % 3;
            return;
        }
        counter++;
    }

    void JumpingProcess()
    {
        if (rb.velocity.y < 0 && jumpCount == 0)
        {
            act = GlidingProcess;
            sr.sprite = s[GlideSprite];
        }
    }

    void GlidingProcess()
    {
        if (!damaged)
        {
            rb.AddForce(Vector2.down * FloatingPower * rb.velocity.y);
            if (miko.transform.position.x < transform.position.x)
            {
                rb.AddForce((rb.velocity.x < 0 ? HighJumpPower / 5 : HighJumpPower) * Vector2.left);
                if (sr.flipX)
                {
                    sr.flipX = false;
                    ColliderFlip(false);
                }
            }
            else
            {
                //Pause();
                rb.AddForce((rb.velocity.x > 0 ? HighJumpPower / 5 : HighJumpPower) * Vector2.right);
                if (!sr.flipX)
                {
                    sr.flipX = true;
                    ColliderFlip(true);
                }
            }
        }
    }

    void SlidingProcess()
    {
        if (counter == 20)
        {
            act = SquatingProcess;
            counter = 0;
            rb.velocity = Vector2.zero;
            return;
        }
        rb.velocity *= 0.9f;
        counter++;
    }

    void MouseEvent()
    {
        if (!invincible && inRange && Input.GetMouseButtonDown(0))
        {
            if (hitArea.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                if (!GameSystem.Instance.Stop)
                {
                    damaged = true;
                    StartCoroutine(CoroutineNameDamageEffect);
                    Damage(miko.KamiAttack);
                    rb.velocity = Vector2.zero;
                    //Debug.Log(HP);
                    if (hp <= 0)
                    {
                        stun = true;
                        act = null;
                        sr.sprite = s[StunSprite];
                        ActiveStunEffect(true);
                        //rb.bodyType = RigidbodyType2D.Dynamic;
                        StartCoroutine(CoroutineNameRevival);
                    }
                }
            }
        }
    }

    void Jump()
    {
        if (miko.transform.position.x < transform.position.x)
        {
            jumpPower.x = -JumpPowerX;
            if (sr.flipX)
            {
                sr.flipX = false;
                ColliderFlip(false);
            }
        }
        else
        {
            jumpPower.x = JumpPowerX;
            if (!sr.flipX)
            {
                sr.flipX = true;
                ColliderFlip(true);
            }
        }
        sr.sprite = s[JumpSprite];
        act = JumpingProcess;
        jumpPower.y = jumpCount == 2 ? HighJumpPower : JumpPower;
        rb.AddForce(jumpPower, ForceMode2D.Impulse);
        jumpCount = (jumpCount + 1) % 3;
    }

    void ColliderFlip(bool b)
    {
        if (b)
        {
            tmp.Set(-ColliderOffset.x, ColliderOffset.y);
            cc.offset = tmp;
            hitArea.offset = tmp;
            tmp.y = BoxColliderOffsetY;
            bc.offset = tmp;
        }
        else
        {
            tmp.Set(ColliderOffset.x, ColliderOffset.y);
            cc.offset = tmp;
            hitArea.offset = tmp;
            tmp.y = BoxColliderOffsetY;
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
                sr.sprite = s[SquatSprite];
                if (jumpCount == 0)
                {
                    act = SlidingProcess;
                }
                else
                {
                    rb.velocity = Vector2.zero;
                    act = SquatingProcess;
                }
                if (damaged)
                {
                    damaged = false;
                }
            }
        }
    }

    private IEnumerator RevivalCoroutine()
    {
        yield return GameSystem.Instance.FiveSecond;
        ActiveStunEffect(false);
        stun = false;
        Revival();
        sr.sprite = s[SquatSprite];
        act = SquatingProcess;
        jumpCount = 0;
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
