using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikochanJumpTrigger : MonoBehaviour
{
    Mikochan mikochan;
    // Start is called before the first frame update
    void Start()
    {
        mikochan = Mikochan.Instance;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground" && !mikochan.IsGround)
            mikochan.IsGround = true;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground" && !mikochan.IsGround)
            mikochan.IsGround = true;
    }
}
