using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikochanJumpTrigger : MonoBehaviour
{
    Mikochan mikochan;
    // Start is called before the first frame update
    void Start()
    {
        mikochan = transform.parent.GetComponent<Mikochan>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log(true);
        if (other.gameObject.tag == "Ground")
        {
            if (!mikochan.IsGround)
                mikochan.IsGround = true;
        }
        //Debug.Log(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(true);
        if (other.gameObject.tag == "Ground")
        {
            //rb.velocity = Vector2.zero;
            if (!mikochan.IsGround)
                mikochan.IsGround = true;
        }
    }
}
