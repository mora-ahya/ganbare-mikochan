using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikochanTrigger : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb = default;
    [SerializeField] CapsuleCollider2D cc = default;
    Vector2 knockBack = new Vector2(0, 3f);
    Enemy target = null;
    Mikochan mikochan;

    private float knockBackDir = 10f;
    private bool inTrap = false;
    // Start is called before the first frame update
    void Start()
    {
        mikochan = Mikochan.Instance;
        knockBack.x = knockBackDir;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyDamage();
        EnemySealed();
        TrapDamage();
    }

    void EnemyDamage()
    {
        if (target == null || target.Stun)
            return;

        if (mikochan.Damaged || mikochan.Invincible)
            return;

        rb.velocity = Vector2.zero;
        mikochan.ChangeHp(-target.Attack);
        knockBack.x = cc.offset.x > 0 ? -knockBackDir : knockBackDir;
        rb.AddForce(knockBack, ForceMode2D.Impulse);
        mikochan.Invincible = true;
        mikochan.Damaged = true;
        mikochan.DoIEnumerator("Inv");
        mikochan.DoIEnumerator("KnockBack");
    }

    void EnemySealed()
    {
        if (target == null || !mikochan.Squat)
            return;

        mikochan.GetExp(target.Exp);
        target.ResetPos();
        target.gameObject.SetActive(false);
        target.ResetFlag = true;
        target = null;
    }

    void TrapDamage()
    {
        if (!inTrap || mikochan.Damaged || mikochan.Invincible)
            return;

        rb.velocity = Vector2.zero;
        mikochan.TrapDamage(0.05f);
        knockBack.x = cc.offset.x > 0 ? -knockBackDir : knockBackDir;
        rb.AddForce(knockBack, ForceMode2D.Impulse);
        mikochan.Invincible = true;
        mikochan.Damaged = true;
        mikochan.DoIEnumerator("Inv");
        mikochan.DoIEnumerator("KnockBack");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            target = other.gameObject.GetComponent<Enemy>();
            return;
        }

        if (other.gameObject.CompareTag("Trap"))
        {
            inTrap = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            if (target != null && target.gameObject == other.gameObject)
            {
                target = null;
            }
            return;
        }

        if (other.gameObject.CompareTag("Trap"))
        {
            inTrap = false;
        }
    }
}
