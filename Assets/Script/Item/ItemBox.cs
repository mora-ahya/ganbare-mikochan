using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour
{
    static Sprite defaultSprite = null;

    Item item = null;
    int amount = 0;

    public int Amount => amount;
    public Item Content => item;

    [SerializeField] Image itemImage = default;
    [SerializeField] Text amountText = default;

    public void Initialize()
    {
        gameObject.SetActive(false);
        if (defaultSprite != null)
            return;

        defaultSprite = itemImage.sprite;
    }

    public void Clear()
    {
        item = null;
        amount = 0;
        itemImage.sprite = defaultSprite;
        gameObject.SetActive(false);
    }

    public void SetItem(Item i, int num)
    {
        gameObject.SetActive(true);
        item = i;
        itemImage.sprite = item.ItemSprite;
        amount = num;
        amountText.text = amount.ToString();
    }

    public void InheritItem(ItemBox other)
    {
        item = other.item;
        amount = other.amount;
        itemImage.sprite = item.ItemSprite;
        amountText.text = other.amountText.text;
    }

    public void SwapItem(ItemBox other)
    {
        if (this == other)
            return;

        (item, other.item) = (other.item, item);
        (amount, other.amount) = (other.amount, amount);
        itemImage.sprite = item.ItemSprite;
        other.itemImage.sprite = other.item.ItemSprite;
        (amountText.text, other.amountText.text) = (other.amountText.text, amountText.text);
    }

    public bool CanContainItem(Item i)
    {
        return item != null && amount <= item.UpperLimit && item.ItemID == i.ItemID;
    }

    public int AdditionalAmount()
    {
        return item.UpperLimit - amount;
    }

    public void IncreaseItem(int amo)
    {
        amount += amo;
        amountText.text = amount.ToString();
    }

    public void DecreaseItem(int amo)
    {
        amount -= amo;
        amountText.text = amount.ToString();
        if (amount > 0)
            return;

        Menu.Instance.ShiftItemBoxes(this);
    }

    public void Clicked()
    {
        Menu.Instance.SetSelectedItemBox(this);
    }

    public void UseItem()
    {
        item.Effect();
        DecreaseItem(1);
    }
}
