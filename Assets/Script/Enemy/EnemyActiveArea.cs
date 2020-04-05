using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActiveArea : MonoBehaviour
{
    const string targetTag = "mikochan";

    [SerializeField] Enemy self = default;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            if (!self.gameObject.activeSelf)
            {
                self.gameObject.SetActive(true);
                self.RestorePhysic();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            if (self.gameObject.activeSelf)
            {
                if (self.Stun)
                {
                    self.ResetFlag = true;
                }
                else
                {
                    self.StorePhysic();
                }
                self.gameObject.SetActive(false);
            }
        }
    }
}
