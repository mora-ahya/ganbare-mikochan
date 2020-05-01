using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [SerializeField] CircleCollider2D cc = default;
    [SerializeField] SpriteRenderer sr = default;
    [SerializeField] Sprite s = default;
    [SerializeField] int amount = default;
    //[SerializeField] Item content = default; //内容物数字

    bool canOpen = false;
    bool empty = false;

    public void Set()
    {

    }

    public void Act()
    {
        if (!canOpen || empty)
            return;

        if (Input.GetMouseButtonDown(0) && cc.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {

            empty = true;
            sr.sprite = s;
            EventTextManager.Instance.Set("霊力を" + amount.ToString() + "Pを手に入れた");
            Mikochan.Instance.GetExp(amount);

        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Mikochan.Instance.tag))
        {
            canOpen = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Mikochan.Instance.tag))
        {
            canOpen = false;
        }
    }
}
