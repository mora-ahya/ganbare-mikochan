using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActiveArea : MonoBehaviour
{
    readonly string targetTag = "mikochan";

    [SerializeField] Enemy self = default;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(targetTag) || self.gameObject.activeSelf)
            return;

        self.gameObject.SetActive(true);
        self.Set();
        self.RestorePhysic();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(targetTag) || !self.gameObject.activeSelf)
            return;

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
