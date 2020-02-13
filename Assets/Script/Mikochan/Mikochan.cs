using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mikochan : MonoBehaviour
{
    private static Mikochan mikoInstance;

    public static Mikochan Instance => mikoInstance;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] CapsuleCollider2D cc;
    [SerializeField] CapsuleCollider2D cc2;
    [SerializeField] BoxCollider2D bc;
    [SerializeField] StatusManager sm;
    [SerializeField] Material damageEffect;
    [SerializeField] Material defaultM;
    [SerializeField] CameraManager cm;
    [SerializeField] GameSystem gs;
    [SerializeField] GameObject kamisama;

    readonly WaitForSeconds wait = new WaitForSeconds(0.08f);
    WaitForSeconds knockBackTime = new WaitForSeconds(0.3f);
    Vector2 tmp = new Vector2();

    readonly float jumpForce = 11f;
    readonly float jumpThreshold = 1.5f;
    readonly float runSpeed = 5.0f;
    readonly float ccOffsetXY = 0.02f;
    readonly float squatOffsetY = -0.165f;
    readonly float squatSizeY = 0.29f;
    readonly float bcOffsetY = -0.31f;
    readonly float defaultSizeY = 0.58f;
    float kamisamaAreaSize = 3.0f;
    float blessing = 1;
    int kamiAttack = 1;
    int hp = 25;
    int maxHp = 25;
    int bonusHp = 0;
    int invincibleCount = 0;
    int invincibleTime = 25;
    int key = 0;
    int exp = 0; //save対象
    bool isGround;
    bool invincible;
    bool damaged;
    bool squat;
    bool jump = false;
    bool stop = false;
    bool movie = false;

    string state;
    string prevState;

    public bool Damaged {
        get
        {
            return damaged;
        }

        set
        {
            damaged = value;
        }
    }

    public bool Invincible
    {
        get
        {
            return invincible;
        }

        set
        {
            invincible = value;
        }
    }

    public bool Squat => squat;

    public bool IsGround
    {
        get
        {
            return isGround;
        }
        set
        {
            isGround = value;
        }
    }

    public bool Movie
    {
        get
        {
            return movie;
        }
        set
        {
            movie = value;
        }
    }

    public int HP => hp;

    public int MaxHP => maxHp + bonusHp;

    public int Exp => exp;

    public int KamiAttack => kamiAttack;

    void Awake()
    {
        mikoInstance = this;
    }

    void Start()
    {
        damaged = false;
        invincible = false;
        squat = false;
        invincibleCount = 0;
        kamiAttack = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(state);
        if (!GameSystem.stop || stop)
        {
            if (!movie)
            {
                GetInputKey();
            }
            ChangeState();
            ChangeAnimation();
            cm.Set(transform.position.x, transform.position.y);
        }
        //Debug.Log(squat);
        //rb.AddForce(transform.right * 10f);
    }

    void FixedUpdate()
    {
        Move();
    }

    void GetInputKey()
    {
        key = 0;
        if (!damaged)
        {
            if (Input.GetKey(KeyCode.S) && isGround)
            {
                squat = true;
                if (cc.size.y == defaultSizeY)
                {
                    tmp.x = cc.offset.x;
                    tmp.y = squatOffsetY;
                    cc.offset = tmp;
                    cc2.offset = tmp;
                    tmp.x = cc.size.x;
                    tmp.y = squatSizeY;
                    cc.size = tmp;
                    cc2.size = tmp;
                }
            }
            else
            {
                squat = false;
                if (cc.size.y == squatSizeY)
                {
                    tmp.x = cc.offset.x;
                    tmp.y = -ccOffsetXY;
                    cc.offset = tmp;
                    cc2.offset = tmp;
                    tmp.x = cc.size.x;
                    tmp.y = defaultSizeY;
                    cc.size = tmp;
                    cc2.size = tmp;
                }
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                cm.LookUnder(true);
            }
            else if (cm.GetLook)
            {
                cm.LookUnder(false);
            }
            //Debug.Log(squat);
            if (!squat)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    key = 1;
                    sr.flipX = false;
                    if (cc.offset.x < 0)
                    {
                        //Debug.Log(true);
                        tmp.x = ccOffsetXY;
                        tmp.y = -ccOffsetXY;
                        cc.offset = tmp;
                        cc2.offset = tmp;
                        tmp.y = bcOffsetY;
                        bc.offset = tmp;
                    }
                }
                if (Input.GetKey(KeyCode.A))
                {
                    key = -1;
                    sr.flipX = true;
                    if (cc.offset.x > 0)
                    {
                        //Debug.Log(true);
                        tmp.x = -ccOffsetXY;
                        tmp.y = -ccOffsetXY;
                        cc.offset = tmp;
                        cc2.offset = tmp;
                        tmp.y = bcOffsetY;
                        bc.offset = tmp;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.W) && isGround)
            {
                jump = true;
            }
        }
    }

    void ChangeState()
    {
        if (Mathf.Abs(rb.velocity.y) >= jumpThreshold)
        {
            isGround = false;
        }

        if (isGround)
        {
            if (squat)
            {
                state = "SQUAT";
            }
            else
            {
                if (key != 0)
                {
                    state = "RUN";
                }
                else
                {
                    state = "IDLE";
                }
            }
        }
        else
        {
            if (rb.velocity.y > 0)
            {
                state = "JUMP";
            }
            else if(rb.velocity.y < 0)
            {
                state = "FALL";
            }
        }
    }

    void ChangeAnimation()
    {
        if (prevState != state)
        {
            switch(state)
            {
                case "JUMP":
                    animator.SetBool("isRun", false);
                    animator.SetBool("isFall", false);
                    animator.SetBool("isJump", true);
                    animator.SetBool("isSquat", false);
                    break;
                case "FALL":
                    animator.SetBool("isRun", false);
                    animator.SetBool("isFall", true);
                    animator.SetBool("isJump", false);
                    animator.SetBool("isSquat", false);
                    break;
                case "RUN":
                    animator.SetBool("isRun", true);
                    animator.SetBool("isFall", false);
                    animator.SetBool("isJump", false);
                    animator.SetBool("isSquat", false);
                    break;
                case "SQUAT":
                    animator.SetBool("isRun", false);
                    animator.SetBool("isFall", false);
                    animator.SetBool("isJump", false);
                    animator.SetBool("isSquat", true);
                    break;
                default:
                    animator.SetBool("isRun", false);
                    animator.SetBool("isFall", false);
                    animator.SetBool("isJump", false);
                    animator.SetBool("isSquat", false);
                    break;
            }
            prevState = state;
        }
    }

    void Move()
    {
        if (!damaged)
        {
            rb.velocity = new Vector2(runSpeed * key, rb.velocity.y);
            if (isGround && jump)
            {
                rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                isGround = false;
                jump = false;
            }
        }
        //rb.velocity = new Vector2(runSpeed * key, rb.velocity.y);
    }

    public void DoIEnumerator(string coroutineName)
    {
        StartCoroutine(coroutineName);
    }

    public void IncreaseMikochanHP(int amount)
    {
        maxHp += amount;
        hp += amount;
        sm.updateHP();
    }

    public void ExtendKamiArea()
    {
        kamisamaAreaSize += 0.3f;
        tmp.Set(kamisamaAreaSize, kamisamaAreaSize);
        kamisama.transform.localScale = tmp;
    }

    public void ChangeHp(int amount)
    {
        if (amount < 0)
        {
            amount = (int)Mathf.Ceil(amount * blessing);
        }
        hp += amount;
        if (hp > MaxHP)
        {
            hp = MaxHP;
        }
        sm.updateHP();
        if (hp <= 0)
        {
            gs.GameOver();
        }
    }

    public void Stop(bool b)
    {
        stop = b;
    }

    public void GetExp(int e)
    {
        exp += e;
        if (exp > 99999)
        {
            exp = 99999;
        }
        sm.updateExp();
    }

    public void TrapDamage(float per)
    {
        ChangeHp(-(int)Mathf.Ceil(maxHp * per));
    }

    public void SetKey(int k)
    {
        key = k;
    }

    public void Special(int specialNum)
    {
        switch (specialNum)
        {
            case 1:
                bonusHp = 10;
                ChangeHp(10);
                break;
            case 2:
                bonusHp = 30;
                ChangeHp(20);
                break;
            case 3:
                bonusHp = 60;
                ChangeHp(30);
                break;
            case 4:
                bonusHp = 100;
                ChangeHp(40);
                break;
            case 5:
                bonusHp = 150;
                ChangeHp(50);
                break;
            case 6:
            case 8:
                kamiAttack += 1;
                break;
            case 7:
            case 9:
                blessing -= 0.1f;
                break;
            case 10:
                kamiAttack += 2;
                break;
            case 11:
                knockBackTime = new WaitForSeconds(0.2f);
                break;
            case 12:
                break;
            case 13:
                invincibleTime = 49;
                break;
            case 14:
                blessing -= 0.3f;
                break;
            case 15:
                knockBackTime = new WaitForSeconds(0.1f);
                break;
        }
    }

    IEnumerator Inv()
    {
        sr.material = damageEffect;
        yield return null;
        yield return null;
        yield return null;
        sr.material = defaultM;
        while (invincibleCount <= invincibleTime)
        {
            sr.enabled = !sr.enabled;
            //sr.color = Color.clear;
            //Debug.Log(true);
            yield return wait;
            invincibleCount++;
        }
        invincible = false;
        invincibleCount = 0;
    }

    IEnumerator KnockBack()
    {
        yield return knockBackTime;
        damaged = false;
    }
}
