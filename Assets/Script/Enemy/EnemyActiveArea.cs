using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActiveArea : MonoBehaviour
{
    const string targetTag = "mikochan";

    [SerializeField] Rigidbody2D self;
    [SerializeField] Enemy self2;

    Vector3 v;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            if (!self.gameObject.activeSelf)
            {
                self.gameObject.SetActive(true);
                self.velocity = v;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            if (self.gameObject.activeSelf)
            {
                if (self2.Stun)
                {
                    self2.ResetFlag = true;
                    v = Vector3.zero;
                }
                else
                {
                    v = self.velocity;
                }
                self.gameObject.SetActive(false);
            }
        }
    }
}
