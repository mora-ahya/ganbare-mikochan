using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VanishWall : MonoBehaviour
{
    [SerializeField] TilemapRenderer tr = default;
    [SerializeField] CompositeCollider2D cc = default;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("mikochan"))
        {
            tr.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("mikochan"))
        {
            tr.enabled = true;
        }
    }
}
