using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GashaShot : Enemy
{
    [SerializeField] Transform gashaHead;//8.0f
    [SerializeField] Gashadokuro gasha;
    [SerializeField] CircleCollider2D cc;
    [SerializeField] Rigidbody2D rb;
    readonly WaitForSeconds stopTime = new WaitForSeconds(0.5f);

    float v = 1.0f;
    Mikochan miko;

    // Start is called before the first frame update
    void Start()
    {
        miko = Mikochan.Instance;
    }

    public void Set(float sv, int maxhp)
    {
        transform.position = gashaHead.position;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        ResetM();
        mhp = maxhp;
        Revival();
        stun = false;
        v = sv;
    }

    // Update is called once per frame
    void Update()
    {
        if (!invincible)
        {
            if (inRange && Input.GetMouseButtonDown(0))
            {
                //Debug.Log(self.InRange);
                if (cc.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                {
                    MouseEvent();
                }
            }
        }
        if (!stun)
        {
            DecideDir();
        }
        else if (hp <= 0)
        {
            FlyAway();
        }
        //Debug.Log(gashaHead.position);
    }

    void DecideDir()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce((miko.transform.position - transform.position).normalized * v, ForceMode2D.Impulse);
    }

    void MouseEvent()
    {
        //Debug.Log(self.InRange);
        if (!GameSystem.stop)
        {
            if (!stun)
            {
                stun = true;
                Damage(miko.KamiAttack);
                rb.velocity *= -4;
                //Debug.Log(self.HP);
                StartCoroutine("DamageEffects");
                if (hp > 0)
                {
                    StartCoroutine("Stop");
                }
                //rb.velocity *= -4;

            }
        }
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

    private IEnumerator DamageEffects()
    {
        DamageEffect();
        yield return null;
        ResetM();
    }

    private IEnumerator Stop()
    {
        yield return stopTime;
        stun = false;

    }

    void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log(true);
        if (other.gameObject.CompareTag("GashaHead"))
        {
            //Debug.Log(true);
            if (hp <= 0)
            {
                gasha.Damage();
                gameObject.SetActive(false);
            }
        }
    }
}
