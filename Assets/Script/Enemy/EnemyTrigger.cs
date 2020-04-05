using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb = default;
    [SerializeField] SpriteRenderer sr = default;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            rb.AddForce(new Vector2(1f, 0), ForceMode2D.Impulse);
            sr.material.SetInt("_Light", 1);
        }
        //Debug.Log(rb.velocity.x);
    }
    /*
    public void OnMouseOver()
    {
        Debug.Log(inRange);
        if (!GameSystem.stop)
        {
            if (inRange && !stun && Input.GetMouseButtonDown(0))
            {
                hp--;
                Debug.Log(hp);
                if (hp <= 0)
                {
                    stun = true;
                    _animator.SetBool("isStun", true);
                    StartCoroutine("Revival");
                    transform.Rotate(0, 0, 90);
                }
            }
        }
    }*/
}
