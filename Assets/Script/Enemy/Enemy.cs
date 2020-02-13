using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int mhp = 3;
    [SerializeField] protected int hp = 3;
    [SerializeField] protected int exp = 1;
    [SerializeField] protected int attack = 1;
    [SerializeField] protected bool invincible = false;
    [SerializeField] protected Material damageEffect;
    [SerializeField] protected Material outLine;
    [SerializeField] protected Material defaultM;
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected Anima2D.SpriteMeshInstance smi;
    [SerializeField] protected EnemyActiveArea eaa;
    [SerializeField] protected Transform defaultPos;
    [SerializeField] protected GameObject stunEffect;

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
        }
        else if (smi != null)
        {
            smi.sharedMaterial = damageEffect;
        }
    }

    public void OutLine()
    {
        if (sr != null)
        {
            sr.material = outLine;
        }
        else if (smi != null)
        {
            smi.sharedMaterial = outLine;
        }
    }

    public void ResetM()
    {
        if (InRange)
        {
            OutLine();
        }
        else
        {
            if (sr != null)
            {
                sr.material = defaultM;
            }
            else if (smi != null)
            {
                smi.sharedMaterial = defaultM;
            }
        }
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
