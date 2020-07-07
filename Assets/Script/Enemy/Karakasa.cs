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
    Vector2 tmp = new Vector2();
    Vector2 jumpPower = new Vector2();

    // Start is called before the first frame update
    public override void Set()
    {
        if (!resetFlag)
            return;

        sr.sprite = s[SquatSprite];
        act = SquatingProcess;
        jumpCount = 1;
        base.Set();
    }

    public override void Act()
    {
        MouseEvent();
        act();
        hitArea.transform.position = transform.position;
        SetActiveAreaPosition();
    }

    void SquatingProcess()
    {
        if (counter++ != 30)
            return;

        counter = 0;
        if (damaged)
        {
            damaged = false;
        }

        Jump();
    }

    void Jump()
    {
        if (Mikochan.Instance.transform.position.x < transform.position.x)
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
        if (damaged)
            return;

        rb.AddForce(Vector2.down * FloatingPower * rb.velocity.y);
        if (Mikochan.Instance.transform.position.x < transform.position.x)
        {
            rb.AddForce((rb.velocity.x < 0 ? HighJumpPower / 5 : HighJumpPower) * Vector2.left);
            if (sr.flipX)
            {
                sr.flipX = false;
                ColliderFlip(false);
            }
            return;
        }

        //Pause();
        rb.AddForce((rb.velocity.x > 0 ? HighJumpPower / 5 : HighJumpPower) * Vector2.right);
        if (!sr.flipX)
        {
            sr.flipX = true;
            ColliderFlip(true);
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

    void StunProcess()
    {
        if (Mathf.Abs(rb.velocity.x) >= 0.01f)
        {
            tmp.Set(0f, rb.velocity.y);
            rb.velocity = tmp;
        }

        if (counter++ < 300 || isSealed)
            return;

        ActiveStunEffect(false);
        stun = false;
        Revival();
        sr.sprite = s[SquatSprite];
        act = SquatingProcess;
        jumpCount = 0;
        counter = 0;
    }

    void MouseEvent()
    {
        if (stun || invincible || !inRange || !Input.GetMouseButtonDown(0) || !hitArea.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            return;

        CameraManager.Instance.MainPostEffect.GenerateRipple(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.05f, 1f, 0.5f, 0f, 1f, 0.5f);
        damaged = true;
        StartCoroutine(CoroutineNameDamageEffect);
        Damage(Mikochan.Instance.KamiAttack);
        rb.velocity = Vector2.zero;

        if (hp <= 0)
        {
            stun = true;
            act = StunProcess;
            sr.sprite = s[StunSprite];
            ActiveStunEffect(true);
            counter = 0;
            //rb.bodyType = RigidbodyType2D.Dynamic;
        }
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
            return;
        }

        tmp.Set(ColliderOffset.x, ColliderOffset.y);
        cc.offset = tmp;
        hitArea.offset = tmp;
        tmp.y = BoxColliderOffsetY;
        bc.offset = tmp;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(true);
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (stun)
                return;

            if (damaged)
            {
                damaged = false;
            }

            sr.sprite = s[SquatSprite];
            if (jumpCount == 0 && act == GlidingProcess)
            {
                act = SlidingProcess;
                counter = 0;
                return;
            }

            if (act == JumpingProcess)
            {
                rb.velocity = Vector2.zero;
                act = SquatingProcess;
            }
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
