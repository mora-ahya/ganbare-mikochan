using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MikochanTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            Mikochan.Instance.Target = other.gameObject.GetComponent<Enemy>();
            return;
        }

        if (other.gameObject.CompareTag("Trap"))
        {
            Mikochan.Instance.InTrap = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            if (Mikochan.Instance.Target != null && Mikochan.Instance.Target.gameObject == other.gameObject)
            {
                Mikochan.Instance.Target = null;
            }
            return;
        }

        if (other.gameObject.CompareTag("Trap"))
        {
            Mikochan.Instance.InTrap = false;
        }
    }
}
