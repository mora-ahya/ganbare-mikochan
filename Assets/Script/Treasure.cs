using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [SerializeField] CircleCollider2D cc;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Sprite s;
    [SerializeField] int amount;

    bool canOpen = false;
    bool empty = false;

    void Update()
    {
        if (canOpen && !empty)
        {
            if (cc.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition))){
                if (Input.GetMouseButtonDown(0))
                {
                    empty = true;
                    sr.sprite = s;
                    EventTextManager.Instance.Set("霊力を" + amount.ToString() + "Pを手に入れた");
                    Mikochan.Instance.GetExp(amount);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Mikochan.Instance.tag))
        {
            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Mikochan.Instance.tag))
        {
            canOpen = false;
        }
    }
}
