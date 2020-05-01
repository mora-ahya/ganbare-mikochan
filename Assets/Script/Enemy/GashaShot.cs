using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GashaShot : Enemy
{
    static readonly string TagNameGashaHead = "GashaHead";
    static readonly string CoroutineNameStop = "Stop";

    [SerializeField] Transform gashaHead = default;//8.0f
    [SerializeField] Gashadokuro gasha = default;
    [SerializeField] CircleCollider2D cc = default;

    float v = 1.0f;

    public override void Set()
    {
        
    }

    public void Set(float sv, int maxhp)
    {
        transform.position = gashaHead.position;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        ResetMaterial();
        mhp = maxhp;
        Revival();
        stun = false;
        act = DecideDir;
        v = sv;
    }

    /*
    void Update()
    {
        MouseEvent();
        act?.Invoke();
    }*/

    public override void Act()
    {
        MouseEvent();
        act?.Invoke();
    }

    void DecideDir()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce((Mikochan.Instance.transform.position - transform.position).normalized * v, ForceMode2D.Impulse);
    }

    void MouseEvent()
    {
        if (stun || GameSystem.Instance.Stop || invincible || !inRange || !Input.GetMouseButtonDown(0) || !cc.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            return;

        stun = true;
        Damage(Mikochan.Instance.KamiAttack);
        rb.velocity *= -4;
        StartCoroutine(CoroutineNameDamageEffect);
        if (hp > 0)
        {
            StartCoroutine(CoroutineNameStop);
            act = null;
            return;
        }
        act = FlyAway;
    }

    void FlyAway()
    {
        rb.AddForce((gashaHead.transform.position - transform.position).normalized, ForceMode2D.Impulse);
        if (rb.velocity.magnitude > 5)
        {
            rb.velocity = rb.velocity.normalized * 5;
        }
        transform.Rotate(0, 0, -20);
    }

    private IEnumerator DamageEffectCoroutine()
    {
        DamageEffect();
        yield return null;
        ResetMaterial();
    }

    private IEnumerator Stop()
    {
        yield return GameSystem.Instance.HalfSecond;
        stun = false;
        act = DecideDir;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(TagNameGashaHead) && hp <= 0)
        {
            gasha.Damage();
            act = null;
            gameObject.SetActive(false);
        }
    }
}
