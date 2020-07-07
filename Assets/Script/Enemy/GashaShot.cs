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
        counter = 0;
    }

    public override void Act()
    {
        MouseEvent();
        act();
    }

    void DecideDir()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce((Mikochan.Instance.transform.position - transform.position).normalized * v, ForceMode2D.Impulse);
    }

    void KnockBack()
    {
        if (counter++ < 30)
            return;

        act = DecideDir;
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

    void MouseEvent()
    {
        if (act != DecideDir || GameSystem.Instance.Stop || !inRange || !Input.GetMouseButtonDown(0) || !cc.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            return;

        CameraManager.Instance.MainPostEffect.GenerateRipple(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.05f, 1f, 0.5f, 0f, 1f, 0.5f);
        counter = 0;
        Damage(Mikochan.Instance.KamiAttack);
        rb.velocity *= -4;
        StartCoroutine(CoroutineNameDamageEffect);
        if (hp > 0)
        {
            act = KnockBack;
            return;
        }
        act = FlyAway;
        stun = true;
        isSealed = true;
    }

    IEnumerator DamageEffectCoroutine()
    {
        DamageEffect();
        yield return null;
        yield return null;
        yield return null;
        ResetMaterial();
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
