using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Koganeko : MonoBehaviour
{
    [SerializeField] Enemy self = default;
    [SerializeField] Animator _animator = default;
    [SerializeField] CapsuleCollider2D cc = default;
    WaitForSeconds stunTime = new WaitForSeconds(5.0f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(self.InRange);
        if (!self.Invincible)
        {
            if (self.InRange && Input.GetMouseButtonDown(0))
            {
                //Debug.Log(self.InRange);
                if (cc.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                {
                    MouseEvent();
                }
            }
        }
    }

    void MouseEvent()
    {
        Debug.Log(self.InRange);
        if (!GameSystem.Instance.Stop)
        {
            if (!self.Stun && Input.GetMouseButtonDown(0))
            {
                StartCoroutine("DamageEffect");
                self.Damage(1);
                Debug.Log(self.HP);
                if (self.HP <= 0)
                {
                    self.Stun = true;
                    _animator.SetBool("isStun", true);
                    StartCoroutine("Revival");
                    transform.Rotate(0, 0, 90);
                }
            }
        }
    }

    private IEnumerator Revival()
    {
        yield return stunTime;
        self.Stun = false;
        _animator.SetBool("isStun", false);
        self.Revival();
        transform.Rotate(0, 0, -90);
    }

    private IEnumerator DamageEffect()
    {
        self.DamageEffect();
        yield return null;
        self.ResetMaterial();
    }
}
