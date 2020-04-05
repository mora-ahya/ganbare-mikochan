using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : Enemy
{
    [SerializeField] BoxCollider2D hitArea = default;
    [SerializeField] Sprite[] s = default;

    void FixedUpdate()
    {
        if (!stun)
        {
            MouseEvent();
        }
    }

    void MouseEvent()
    {
        if (!invincible && inRange && Input.GetMouseButtonDown(0))
        {
            //Debug.Log(self.InRange);
            if (hitArea.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                if (!GameSystem.Instance.Stop)
                {
                    StartCoroutine("DamageEffects");
                    Damage(Mikochan.Instance.KamiAttack);
                    Debug.Log(HP);
                    if (hp <= 0)
                    {
                        stun = true;
                        sr.sprite = s[1];
                        ActiveStunEffect(true);
                        StartCoroutine("Revivals");
                    }
                }
            }
        }
    }

    private IEnumerator Revivals()
    {
        yield return GameSystem.Instance.FiveSecond;
        ActiveStunEffect(false);
        stun = false;
        Revival();
        sr.sprite = s[0];
    }

    private IEnumerator DamageEffects()
    {
        DamageEffect();
        yield return null;
        yield return null;
        yield return null;
        ResetMaterial();
    }
}
