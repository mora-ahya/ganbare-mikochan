using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : Enemy
{
    [SerializeField] BoxCollider2D hitArea = default;
    [SerializeField] Sprite[] s = default;

    public override void Set()
    {
        if (!resetFlag)
            return;

        sr.sprite = s[0];
        base.Set();
    }

    public override void Act()
    {
        if (!stun)
        {
            MouseEvent();
            return;
        }

        act();
    }

    void MouseEvent()
    {
        if (invincible || !inRange || !Input.GetMouseButtonDown(0) || !hitArea.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            return;

        StartCoroutine(CoroutineNameDamageEffect);
        Damage(Mikochan.Instance.KamiAttack);
        if (hp <= 0)
        {
            stun = true;
            act = StunProcess;
            sr.sprite = s[1];
            ActiveStunEffect(true);
        }
    }

    void StunProcess()
    {
        if (counter++ < 300)
            return;

        ActiveStunEffect(false);
        stun = false;
        Revival();
        sr.sprite = s[0];
        counter = 0;
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
