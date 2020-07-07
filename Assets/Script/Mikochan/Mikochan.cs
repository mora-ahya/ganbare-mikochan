using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInitSet;

public class Mikochan : MonoBehaviour
{
    private static Mikochan mikoInstance;

    public static Mikochan Instance => mikoInstance;

    readonly string animNameMikochanSeal = "MikochanSeal";
    readonly string animBoolIsRun = "isRun";
    readonly string animBoolIsJump = "isJump";
    readonly string animBoolIsFall = "isFall";
    readonly string animBoolIsSquat = "isSquat";
    readonly string animBoolIsSeal = "isSeal";

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
    [SerializeField] QuickUseItem quickUseItem = default;

    Vector2 tmp = new Vector2();
    Vector2 positionTmp;
    Vector2 storedVelocity;
    Vector2 knockBack = new Vector2(0, 3f);
    Enemy target = null;
    Enemy sealTarget = null;
    FunctionalStateMachine mode;
    AnimatorStateInfo anim;

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
    int invincibleTime = 40;
    int knockBackTime = 30;
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
        Squat,
        UseItem,
        Seal
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

    public bool Squat => squat;

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
        knockBack.x = knockBackDir;
        mode = OperationalMode;
    }

    public void SetActiveQuickUse(bool isActive)
    {
        quickUseItem.gameObject.SetActive(isActive);
        quickUseItem.SetItemInfo();
    }

    //常に実行する関数
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
                tmp.Set(cc.offset.x, squatOffsetY);
                cc.offset = tmp;
                cc2.offset = tmp;
                tmp.Set(cc.size.x, squatSizeY);
                cc.size = tmp;
                cc2.size = tmp;
            }

            if (target != null && target.Stun && !target.IsSealed)
            {
                sealTarget = target;
                target = null;
                sealTarget.IsSealed = true;
                mode = SealMode;
                positionTmp = transform.position;
                Pause();
                Menu.Instance.IsOperational = false;
                tmp = sealTarget.transform.position;
                tmp.x -= 0.5f;
                sr.flipX = false;
                transform.position = tmp;
                GameSystem.Instance.Whiteness.gameObject.SetActive(false);
                return;
            }
        }
        else
        {
            squat = false;
            if (cc.size.y == squatSizeY)
            {
                tmp.Set(cc.offset.x, -ccOffsetXY);
                cc.offset = tmp;
                cc2.offset = tmp;
                tmp.Set(cc.size.x, defaultSizeY);
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
                tmp.Set(ccOffsetXY, -ccOffsetXY);
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
                tmp.Set(-ccOffsetXY, -ccOffsetXY);
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
                state = sealTarget == null || !sealTarget.Stun ? State.Squat : State.Seal;
                return;
            }

            state = key != 0 ? State.Run : State.Idle;
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
                animator.SetBool(animBoolIsRun, false);
                animator.SetBool(animBoolIsFall, false);
                animator.SetBool(animBoolIsJump, true);
                animator.SetBool(animBoolIsSquat, false);
                animator.SetBool(animBoolIsSeal, false);
                break;
            case State.Fall:
                animator.SetBool(animBoolIsRun, false);
                animator.SetBool(animBoolIsFall, true);
                animator.SetBool(animBoolIsJump, false);
                animator.SetBool(animBoolIsSquat, false);
                animator.SetBool(animBoolIsSeal, false);
                break;
            case State.Run:
                animator.SetBool(animBoolIsRun, true);
                animator.SetBool(animBoolIsFall, false);
                animator.SetBool(animBoolIsJump, false);
                animator.SetBool(animBoolIsSquat, false);
                animator.SetBool(animBoolIsSeal, false);
                break;
            case State.Squat:
                animator.SetBool(animBoolIsRun, false);
                animator.SetBool(animBoolIsFall, false);
                animator.SetBool(animBoolIsJump, false);
                animator.SetBool(animBoolIsSquat, true);
                animator.SetBool(animBoolIsSeal, false);
                break;
            case State.Seal:
                animator.SetBool(animBoolIsRun, false);
                animator.SetBool(animBoolIsFall, false);
                animator.SetBool(animBoolIsJump, false);
                animator.SetBool(animBoolIsSquat, false);
                animator.SetBool(animBoolIsSeal, true);
                break;
            default:
                animator.SetBool(animBoolIsRun, false);
                animator.SetBool(animBoolIsFall, false);
                animator.SetBool(animBoolIsJump, false);
                animator.SetBool(animBoolIsSquat, false);
                animator.SetBool(animBoolIsSeal, false);
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

    public void SealCommand()
    {
        GameSystem.Instance.Whiteness.gameObject.SetActive(false);
        animator.SetBool(animBoolIsRun, false);
        animator.SetBool(animBoolIsFall, false);
        animator.SetBool(animBoolIsJump, false);
        animator.SetBool(animBoolIsSquat, false);
        animator.SetBool(animBoolIsSeal, true);
        mode = SealingCommand;
    }

    void SealingCommand()
    {
        anim = animator.GetCurrentAnimatorStateInfo(0);
        if (!anim.IsName(animNameMikochanSeal) || anim.normalizedTime < 1.0f)
            return;

        CameraManager.Instance.MainPostEffect.GenerateRipple(transform.position, 0.1f, 5f, 0.75f, 1.5f, 0.5f, 2.5f);
        GameSystem.Instance.Whiteness.gameObject.SetActive(true);
        GameSystem.Instance.Whiteness.CrossFadeAlpha(0f, 0.2f, false);
        animator.SetBool(animBoolIsSeal, false);
        mode = AutoMode;
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
            amount = (int)(amount * blessing);
            amount = amount == 0 ? -1 : amount;
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
                knockBackTime = 18;
                break;
            case 12:
                invincibleTime = 80;
                break;
            case 13:
                knockBackTime = 0;
                break;
            case 14:
                invincibleTime = 120;
                break;
            case 15:
                blessing -= 0.3f;
                break;
        }
    }

    void EnemyDamage()
    {
        if (target == null || target.Stun || invincible)
            return;

        ChangeHp(-target.Attack);
        if (knockBackTime != 0)
        {
            rb.velocity = Vector2.zero;
            knockBack.x = cc.offset.x > 0 ? -knockBackDir : knockBackDir;
            rb.AddForce(knockBack, ForceMode2D.Impulse);
        }
        invincible = true;
        damaged = true;
        sr.material = damageEffect;
    }

    void TrapDamage()
    {
        if (!inTrap || invincible)
            return;

        rb.velocity = Vector2.zero;
        TrapDamage(0.05f);
        //knockBack.x = cc.offset.x > 0 ? -knockBackDir : knockBackDir;
        //rb.AddForce(knockBack, ForceMode2D.Impulse);
        invincible = true;
        //damaged = true;
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
        TrapDamage();
        InvincibleProcess();
        if (quickUseItem.gameObject.activeSelf)
            quickUseItem.Act();
    }

    void AutoMode()
    {
        ChangeState();
        ChangeAnimation();
        cm.Set(transform.position.x, transform.position.y);
        Move();
    }

    void SealMode()
    {
        anim = animator.GetCurrentAnimatorStateInfo(0);
        if (anim.normalizedTime < 1.0f)
            return;

        CameraManager.Instance.MainPostEffect.GenerateRipple(sealTarget.transform.position, 0.1f, 5f, 0.75f, 1.5f, 0.5f, 2.5f);
        //GameSystem.Instance.Whiteness.gameObject.SetActive(true);
        //GameSystem.Instance.Whiteness.CrossFadeAlpha(0f, 0.2f, false);
        sealTarget.Sealed();
        sealTarget = null;
        mode = OperationalMode;
        Restart();
        Menu.Instance.IsOperational = true;
        transform.position = positionTmp;
        invincible = true;
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

        if (damaged && invincibleCount >= knockBackTime)
        {
            damaged = false;
        }

        if (invincibleCount == 16)
            sr.material = defaultM;

        if (invincibleCount % 8 == 0)
            sr.enabled = !sr.enabled;

        if (invincibleCount++ <= invincibleTime)
            return;

        sr.enabled = true;
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
