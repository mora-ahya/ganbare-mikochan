using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static readonly string TagNameEnemy = "enemy";

    protected static readonly string CoroutineNameDamageEffect = "DamageEffectCoroutine";
    protected static readonly string AnimFloatGameSpeed = "GameSpeed";

    [SerializeField] protected int mhp = 3;
    [SerializeField] protected int hp = 3;
    [SerializeField] protected int exp = 1;
    [SerializeField] protected int attack = 1;
    [SerializeField] protected bool invincible = false;
    [SerializeField] protected Shader whiteOutShader = default;
    [SerializeField] protected Material damageEffect = default;
    [SerializeField] protected Material outLine = default;
    [SerializeField] protected Material defaultM = default;
    [SerializeField] protected SpriteRenderer sr = default;
    [SerializeField] protected Anima2D.SpriteMeshInstance smi = default;
    [SerializeField] protected EnemyActiveArea eaa = default;
    [SerializeField] protected Transform defaultPos = default;
    [SerializeField] protected GameObject stunEffect = default;
    [SerializeField] protected Rigidbody2D rb = default;
    [SerializeField] protected Shinryoku shinryoku = default;
    [SerializeField] protected Animator animator = default;

    Vector2 storedVelocity;
    bool isInit = true;

    protected Vector3 initSize;

    protected Material whiteOutMaterial = null;

    protected bool stun = false;
    protected bool inRange = false;
    protected bool resetFlag = false;
    protected bool isSealed = false;
    protected int counter = 0;
    float whiteDegree = 0;
    protected FunctionalStateMachine act;

    public int Attack => attack;

    public bool Invincible => invincible;

    public int HP => hp;

    public int Exp => exp;

    public bool ResetFlag
    {
        get
        {
            return resetFlag;
        }
        set
        {
            resetFlag = value;
        }
    }

    public bool InRange
    {
        get
        {
            return inRange;
        }
        set
        {
            inRange = value;
        }
    }

    public bool Stun
    {
        get
        {
            return stun;
        }
        set
        {
            stun = value;
        }
    }

    public bool IsSealed
    {
        get
        {
            return isSealed;
        }
        set
        {
            isSealed = value;
        }
    }

    public virtual void Set()
    {
        if (isInit)
        {
            initSize = new Vector3(transform.localScale.x, transform.localScale.y, 1f);
            if (whiteOutShader != default)
                whiteOutMaterial = new Material(whiteOutShader);

            if (shinryoku != default)
                shinryoku.Init(whiteOutMaterial);

            isInit = false;
        }

        stun = false;
        inRange = false;
        Revival();
        counter = 0;
        ResetMaterial();
        ActiveStunEffect(false);
        resetFlag = false;
        isSealed = false;
        transform.localScale = initSize;
        rb.bodyType = RigidbodyType2D.Dynamic;
        storedVelocity = Vector2.zero;
        if (shinryoku.gameObject.activeSelf)
            shinryoku.gameObject.SetActive(false);
    }

    public virtual void Act()
    {

    }

    public void Damage(int amount)
    {
        hp -= amount;
    }

    public void Revival()
    {
        hp = mhp;
    }

    public void DamageEffect()
    {
        if (sr != null)
        {
            sr.material = damageEffect;
            return;
        }

        if (smi != null)
        {
            smi.sharedMaterial = damageEffect;
        }
    }

    public void OutLine()
    {
        if (sr != null)
        {
            sr.material = outLine;
            return;
        }

        if (smi != null)
        {
            smi.sharedMaterial = outLine;
        }
    }

    public void ResetMaterial()
    {
        if (InRange)
        {
            OutLine();
            return;
        }

        if (sr != null)
        {
            sr.material = defaultM;
            return;
        }

        if (smi != null)
        {
            smi.sharedMaterial = defaultM;
        }
    }

    public void Sealed()
    {
        whiteDegree = 0f;
        whiteOutMaterial.SetFloat("_White_Degree", whiteDegree);
        sr.material = whiteOutMaterial;
        counter = 0;
        shinryoku.Set();
        shinryoku.gameObject.transform.position = transform.position;
        act = SealedProcess;
        rb.bodyType = RigidbodyType2D.Static;
    }

    protected virtual void SealedProcess()
    {
        if (transform.localScale.x > 0f)
        {
            if (whiteDegree < 1f && transform.localScale.x >= initSize.x / 2f)
            {
                whiteDegree += 1f / 15f;
                whiteOutMaterial.SetFloat("_White_Degree", whiteDegree);
            }

            transform.localScale -= initSize / 60f;
        }

        if (shinryoku.gameObject.activeSelf)
        {
            shinryoku.Process();
            return;
        }

        if (transform.localScale.x < initSize.x / 2f)
            shinryoku.gameObject.SetActive(true);
    }

    public bool CurrentMaterialIsDamageEffect()
    {
        if (sr != null)
        {
            if (sr.material == damageEffect)
            {
                return true;
            }
        }

        if (smi != null)
        {
            if (smi.sharedMaterial == damageEffect)
            {
                return true;
            }
        }
        return false;
    }

    public virtual void Pause()
    {
        if (rb != null && rb.bodyType != RigidbodyType2D.Static)
        {
            StorePhysic();
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (animator != null)
            animator.SetFloat(AnimFloatGameSpeed, 0f);
    }

    public virtual void Restart()
    {
        if (rb != null && rb.bodyType != RigidbodyType2D.Dynamic)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            RestorePhysic();
        }

        if (animator != null)
            animator.SetFloat(AnimFloatGameSpeed, 1f);
    }

    public virtual void StorePhysic()
    {
        if (rb != null)
        {
            storedVelocity = rb.velocity;
            //rb.velocity = Vector2.zero;
        }
    }

    public virtual void RestorePhysic()
    {
        if (rb != null)
            rb.velocity = storedVelocity;

        storedVelocity = Vector2.zero;
    }

    public void SetActiveAreaPosition()
    {
        eaa.gameObject.transform.position = transform.position;
    }

    public void ResetPos()
    {
        if (defaultPos != null)
        {
            transform.position = defaultPos.position;
            eaa.gameObject.transform.position = defaultPos.position;
        }
    }

    public void ActiveStunEffect(bool b)
    {
        if (stunEffect.activeSelf != b)
        {
            stunEffect.SetActive(b);
        }
    }
}
