using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    static string text1 = "神力を{0}手に入れた";
    static string text2 = "{0}を{1}個手に入れた";
    static Item tmpItem;

    [SerializeField] CircleCollider2D cc = default;
    [SerializeField] SpriteRenderer sr = default;
    [SerializeField] Sprite s = default;
    [SerializeField] int id = default;
    [SerializeField] int amount = default;
    [SerializeField] int containedItemID = default; //内容物数字

    bool canOpen = false;
    bool empty = false;

    void Start()
    {
        empty = !GameSystem.Instance.GetTreasureFlag(id);
    }

    public void Act()
    {
        if (!canOpen || empty)
            return;
        //Debug.Log(true);
        if (Input.GetMouseButtonDown(0) && cc.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {

            empty = true;
            GameSystem.Instance.SetTreasureFlag(id);
            sr.sprite = s;
            if (containedItemID == -1)
            {
                Mikochan.Instance.GetExp(amount);
                EventTextManager.Instance.Set(string.Format(text1, amount));
                return;
            }
            tmpItem = ItemManager.Instance.GetItemInfo(containedItemID);
            EventTextManager.Instance.Set(string.Format(text2, tmpItem.ItemName, amount));
            Menu.Instance.StoreItem(tmpItem, amount);
            tmpItem = null;
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
