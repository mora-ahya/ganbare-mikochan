using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanyudou : Enemy
{
    private readonly int SHOT_COUNT = 4;

    [SerializeField] Animator animator;
    private bool runCoroutine = false;
    State state = State.Appearance;

    enum State
    {
        Appearance,
        Stun,
        Recovery
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Appearance: //登場シーン
                break;

            

           

            

            case State.Stun: //気絶状態
                
                break;

            case State.Recovery: //気絶から復帰
                
                break;
        }
        Move();
        MakeShot();
    }

    

    void Move()
    {
        
    }


    void ReturnDefaultPos() //中心へ戻る動き
    {
        
    }

    void MakeShot() //がしゃショットの有効化
    {
        
    }

    void Upgrade() //一定ダメージごとに攻撃を早くする
    {
        
    }

    private IEnumerator DamageEffects() //ダメージを受けたとき赤くなる
    {
        DamageEffect();
        yield return null;
        yield return null;
        yield return null;
        ResetM();
    }

    private IEnumerator ShotInterval() //ショットの間隔
    {
        yield return null;
    }

    public void Damage() //ダメージを受けたときの処理
    {
        Damage(1);
        StartCoroutine("DamageEffect");
        if (state != State.Stun)
        {
            if (hp <= 0)
            {
                
            }
            else
            {
                
            }
        }
    }
}
