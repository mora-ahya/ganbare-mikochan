using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanyudou : Enemy
{
    private IEnumerator DamageEffects() //ダメージを受けたとき赤くなる
    {
        DamageEffect();
        yield return null;
        yield return null;
        yield return null;
        ResetMaterial();
    }

    public void Damage() //ダメージを受けたときの処理
    {
        Damage(1);
        StartCoroutine("DamageEffect");
    }
}
