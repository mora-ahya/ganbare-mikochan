using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikochanJumpTrigger : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground" && !Mikochan.Instance.IsGround)
            Mikochan.Instance.IsGround = true;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground" && !Mikochan.Instance.IsGround)
            Mikochan.Instance.IsGround = true;
    }
}
