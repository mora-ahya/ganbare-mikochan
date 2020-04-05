using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamisama : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag(Enemy.TagNameEnemy))
            return;

        Enemy tmp = other.gameObject.GetComponent<Enemy>();
        tmp.InRange = true;
        if (!tmp.CurrentMaterialIsDamageEffect())
        {
            tmp.OutLine();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag(Enemy.TagNameEnemy))
            return;

        Enemy tmp = other.gameObject.GetComponent<Enemy>();
        tmp.InRange = false;
        if (!tmp.CurrentMaterialIsDamageEffect())
        {
            tmp.ResetMaterial();
        }
    }
}
