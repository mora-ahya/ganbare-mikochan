using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static readonly string TagNameEnemy = "enemy";

    protected delegate void Action();

    protected static readonly string CoroutineNameRevival = "RevivalCoroutine";
    protected static readonly string CoroutineNameDamageEffect = "DamageEffectCoroutine";

    [SerializeField] protected int mhp = 3;
    [SerializeField] protected int hp = 3;
    [SerializeField] protected int exp = 1;
    [SerializeField] protected int attack = 1;
    [SerializeField] protected bool invincible = false;
    [SerializeField] protected Material damageEffect = default;
    [SerializeField] protected Material outLine = default;
    [SerializeField] protected Material defaultM = default;
    [SerializeField] protected SpriteRenderer sr = default;
    [SerializeField] protected Anima2D.SpriteMeshInstance smi = default;
    [SerializeField] protected EnemyActiveArea eaa = default;
    [SerializeField] protected Transform defaultPos = default;
    [SerializeField] protected GameObject stunEffect = default;
    [SerializeField] protected Rigidbody2D rb = default;

    Vector2 storedVelocity;

    protected bool stun = false;
    protected bool inRange = false;
    protected bool resetFlag = false;

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

    public virtual void Set()
    {

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

    public bool CurrentMaterialIsDamageEffect()
    {
        if (sr != null)
        {
            if (sr.material == damageEffect)
            {
                Debug.Log(true);
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
        if (rb != null)
        {
            StorePhysic();
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    public virtual void Restart()
    {
        if (rb != null)
        {
            RestorePhysic();
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
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
