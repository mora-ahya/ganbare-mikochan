using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mikochan : MonoBehaviour
{
    private static Mikochan mikoInstance;

    public static Mikochan Instance => mikoInstance;

    readonly string animNameIsRun = "isRun";
    readonly string animNameIsJump = "isJump";
    readonly string animNameIsFall = "isFall";
    readonly string animNameIsSquat = "isSquat";

    [SerializeField] Rigidbody2D rb = default;
    [SerializeField] Animator animator = default;
    [SerializeField] SpriteRenderer sr = default;
    [SerializeField] CapsuleCollider2D cc = default;
    [SerializeField] CapsuleCollider2D cc2 = default;
    [SerializeField] BoxCollider2D bc = default;
    [SerializeField] Material damageEffect = default;
    [SerializeField] Material defaultM = default;
    [SerializeField] CameraManager cm = default;
    [SerializeField] GameObject kamisama = default;
    [SerializeField] MyPostEffects mpe = default;

    Vector2 tmp = new Vector2();
    Vector2 storedVelocity;
    Vector2 knockBack = new Vector2(0, 3f);
    Enemy target = null;
    FunctionalStateMachine mode;

    readonly float jumpForce = 11f;
    readonly float jumpThreshold = 1.5f;
    readonly float runSpeed = 5.0f;
    readonly float ccOffsetXY = 0.02f;
    readonly float squatOffsetY = -0.165f;
    readonly float squatSizeY = 0.29f;
    readonly float bcOffsetY = -0.31f;
    readonly float defaultSizeY = 0.58f;
    float kamisamaAreaSize = 3.0f;
    float blessing = 1f;
    float knockBackDir = 10f;
    int kamiAttack = 1;
    int hp = 25;
    int maxHp = 25;
    int bonusHp = 0;
    int invincibleCount = 0;
    int invincibleTime = 120;
    int knockBackTime = 18;
    int key = 0;
    int exp = 0; //save対象
    bool isGround;
    bool invincible;
    bool damaged;
    bool squat;
    bool jump = false;
    bool stop = false;
    bool movie = false;
    bool inTrap = false;

    enum State
    {
        Idle,
        Run,
        Jump,
        Fall,
        Squat
    }

    State state;
    State prevState;

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

    public bool InTrap
    {
        get
        {
            return inTrap;
        }
        set
        {
            inTrap = value;
        }
    }

    public int HP => hp;

    public int MaxHP => maxHp + bonusHp;

    public int Exp => exp;

    public int KamiAttack => kamiAttack;

    public Enemy Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }

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
        knockBack.x = knockBackDir;
        mode = OperationalMode;
    }

    public void Act()
    {
        mode();
    }

    void GetInputKey()
    {
        key = 0;
        if (damaged)
            return;

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
        if (Input.GetKeyDown(KeyCode.W) && isGround)
        {
            jump = true;
        }

        if (squat)
            return;

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
                state = State.Squat;
                return;
            }

            if (key != 0)
            {
                state = State.Run;
            }
            else
            {
                state = State.Idle;
            }
            return;
        }

        if (rb.velocity.y > 0)
        {
            state = State.Jump;
        }
        else if (rb.velocity.y < 0)
        {
            state = State.Fall;
        }
    }

    void ChangeAnimation()
    {
        if (prevState == state)
            return;

        switch (state)
        {
            case State.Jump:
                animator.SetBool(animNameIsRun, false);
                animator.SetBool(animNameIsFall, false);
                animator.SetBool(animNameIsJump, true);
                animator.SetBool(animNameIsSquat, false);
                break;
            case State.Fall:
                animator.SetBool(animNameIsRun, false);
                animator.SetBool(animNameIsFall, true);
                animator.SetBool(animNameIsJump, false);
                animator.SetBool(animNameIsSquat, false);
                break;
            case State.Run:
                animator.SetBool(animNameIsRun, true);
                animator.SetBool(animNameIsFall, false);
                animator.SetBool(animNameIsJump, false);
                animator.SetBool(animNameIsSquat, false);
                break;
            case State.Squat:
                animator.SetBool(animNameIsRun, false);
                animator.SetBool(animNameIsFall, false);
                animator.SetBool(animNameIsJump, false);
                animator.SetBool(animNameIsSquat, true);
                break;
            default:
                animator.SetBool(animNameIsRun, false);
                animator.SetBool(animNameIsFall, false);
                animator.SetBool(animNameIsJump, false);
                animator.SetBool(animNameIsSquat, false);
                break;
        }
        prevState = state;

    }

    void Move()
    {
        if (damaged)
            return;

        tmp.Set(runSpeed * key, rb.velocity.y);
        rb.velocity = tmp;
        if (isGround && jump)
        {
            tmp = transform.up;
            tmp *= jumpForce;
            rb.AddForce(tmp, ForceMode2D.Impulse);
            isGround = false;
            jump = false;
        }
    }

    public void JumpCommand()
    {
        jump = true;
    }

    public void SquatCommand()
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

    public void StandUpCommand()
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

    public void RunCommand(int k)
    {
        key = k > 0 ? 1 : -1;
        sr.flipX = key == -1;
        if (cc.offset.x * key < 0)
        {
            //Debug.Log(true);
            tmp.x = key * ccOffsetXY;
            tmp.y = -ccOffsetXY;
            cc.offset = tmp;
            cc2.offset = tmp;
            tmp.y = bcOffsetY;
            bc.offset = tmp;
        }
    }

    public void StopCommand()
    {
        key = 0;
    }

    public void IncreaseMikochanHP(int amount)
    {
        maxHp += amount;
        hp += amount;
        StatusManager.Instance.updateHP();
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
        StatusManager.Instance.updateHP();
        if (hp <= 0)
        {
            GameSystem.Instance.GameOver();
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
        StatusManager.Instance.updateExp();
    }

    public void TrapDamage(float per)
    {
        ChangeHp(-(int)Mathf.Ceil(maxHp * per));
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
                knockBackTime = 30;
                break;
            case 12:
                break;
            case 13:
                invincibleTime = 180;
                break;
            case 14:
                blessing -= 0.3f;
                break;
            case 15:
                knockBackTime = 18;
                break;
        }
    }

    void EnemyDamage()
    {
        if (target == null || target.Stun || damaged || invincible)
            return;

        rb.velocity = Vector2.zero;
        ChangeHp(-target.Attack);
        knockBack.x = cc.offset.x > 0 ? -knockBackDir : knockBackDir;
        rb.AddForce(knockBack, ForceMode2D.Impulse);
        invincible = true;
        damaged = true;
        sr.material = damageEffect;
    }

    void SealingEnemy()
    {
        if (target == null || !squat || !target.Stun)
            return;

        /*
        GetExp(target.Exp);
        target.ResetPos();
        target.gameObject.SetActive(false);
        target.ResetFlag = true;
        */
        target.IsSealed = true;
        target.Sealed();
        target = null;
    }

    void TrapDamage()
    {
        if (!inTrap || damaged || invincible)
            return;

        rb.velocity = Vector2.zero;
        TrapDamage(0.05f);
        knockBack.x = cc.offset.x > 0 ? -knockBackDir : knockBackDir;
        rb.AddForce(knockBack, ForceMode2D.Impulse);
        invincible = true;
        damaged = true;
        sr.material = damageEffect;
    }

    void OperationalMode()
    {
        if (GameSystem.Instance.Stop)
            return;

        GetInputKey();
        ChangeState();
        ChangeAnimation();
        cm.Set(transform.position.x, transform.position.y);

        Move();
        EnemyDamage();
        SealingEnemy();
        TrapDamage();
        InvincibleProcess();
    }

    void AutoMode()
    {
        ChangeState();
        ChangeAnimation();
        cm.Set(transform.position.x, transform.position.y);
        Move();
    }

    public void ChangeAuto()
    {
        mode = AutoMode;
    }

    public void ChangeOperational()
    {
        mode = OperationalMode;
    }

    public void Pause()
    {
        StorePhysic();
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
    }

    public void Restart()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 2f;
        RestorePhysic();
    }

    public void StorePhysic()
    {
        storedVelocity = rb.velocity;
        //rb.velocity = Vector2.zero;
    }

    public void RestorePhysic()
    {
        rb.velocity = storedVelocity;
        storedVelocity = Vector2.zero;
    }
    /*//テスト実装
    public void TimeStop()
    {
        animator.SetBool("isCharge", true);
        StartCoroutine(TimeStopCroutine());
    }
    */

    void InvincibleProcess()
    {
        if (!invincible)
            return;

        if (invincibleCount == knockBackTime)
        {
            damaged = false;
        }

        if (invincibleCount == 10)
            sr.material = defaultM;

        if (invincibleCount % 8 == 0)
            sr.enabled = !sr.enabled;

        if (invincibleCount++ <= invincibleTime)
            return;

        sr.material = defaultM;
        invincible = false;
        invincibleCount = 0;
    }

    /*//テスト実装
    IEnumerator TimeStopCroutine()
    {
        yield return oneSecond;
        mpe.SetEffectActive(MyPostEffects.CIRCLE_GRAYSCALE_EFFECT, true);
        yield return oneSecond;
        animator.SetBool("isCharge", false);

    }*/
}
