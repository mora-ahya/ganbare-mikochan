using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PossessionItem : MonoBehaviour
{
    readonly string itemExpain = "{0} :\n{1}";
    readonly string organizeText = "統合/交換したいアイテムを選択してください";
    readonly string throwAwayText = "{0}をいくつすてますか";

    [SerializeField] ItemBox[] itemBoxes = default;
    [SerializeField] GameObject choices = default;
    [SerializeField] GameObject throwAwayUI = default;
    [SerializeField] Text explain = default;
    [SerializeField] Text throwAwayAmountText = default;
    [SerializeField] Transition transition = default;

    //左上572, 422.5

    Vector3 positionTmp;
    ItemBox itemBoxTmp;
    ItemBox selectedItemBox;
    int itemAmount = 0;
    int throwAwayAmount = 0;

    public int ItemAmount => itemAmount;

    public void Initialize()
    {
        foreach (ItemBox ib in itemBoxes)
        {
            ib.Initialize();
        }
    }

    public void StoreItem(Item item, int amount)
    {
        for (int i = 0; i < itemAmount; i++)
        {
            itemBoxTmp = itemBoxes[i];
            if (!itemBoxTmp.CanContainItem(item))
                continue;

            if (itemBoxTmp.AdditionalAmount() >= amount)
            {
                itemBoxTmp.IncreaseItem(amount);
                itemBoxTmp = null;
                return;
            }

        }

        itemBoxTmp = null;
        if (itemAmount > 15)
            return;

        itemBoxes[itemAmount].SetItem(item, amount);
        itemAmount++;
    }

    public void SetSelectedItemBox(ItemBox itemBox)
    {
        if (throwAwayUI.activeSelf)
            throwAwayUI.SetActive(false);

        if (itemBoxTmp != null)
        {
            OrganizeProcess(itemBox);
            itemBoxTmp = null;
            selectedItemBox = null;
            explain.text = null;
            return;
        }

        if (selectedItemBox == itemBox)
        {
            choices.SetActive(false);
            selectedItemBox = null;
            explain.text = null;
            return;
        }

        selectedItemBox = itemBox;
        explain.text = string.Format(itemExpain, itemBox.Content.ItemName, itemBox.Content.Explain);
        positionTmp = Camera.main.WorldToScreenPoint(selectedItemBox.transform.position);
        positionTmp.x += positionTmp.x < 712f ? 80f : -80;
        choices.transform.position = Camera.main.ScreenToWorldPoint(positionTmp);
        choices.SetActive(true);
    }

    public ItemBox GetItemBoxInfo(int num)
    {
        return itemBoxes[num].Content == null ? null : itemBoxes[num];
    }

    public void ShiftItemBoxes(ItemBox tmp)
    {
        itemAmount--;
        itemBoxTmp = null;
        int i;

        for (i = 0; i < itemAmount; i++)
        {
            if (itemBoxes[i] == tmp)
            {
                itemBoxTmp = tmp;
                itemBoxTmp.InheritItem(itemBoxes[i + 1]);
                continue;
            }

            if (itemBoxTmp == null)
                continue;

            itemBoxes[i].InheritItem(itemBoxes[i + 1]);
                
        }
        itemBoxes[itemAmount].Clear();
        itemBoxTmp = null;
    }

    public void UseItem()
    {
        selectedItemBox.UseItem();
        explain.text = null;
        selectedItemBox = null;
        choices.SetActive(false);
        transition.HealEffect();
    }

    public void ThrowAwayItem()
    {
        throwAwayAmount = 0;
        throwAwayAmountText.text = throwAwayAmount.ToString();
        explain.text = string.Format(throwAwayText, selectedItemBox.Content.ItemName);
        throwAwayUI.SetActive(true);
        choices.SetActive(false);
    }

    public void IncreaseThrowAwayAmount()
    {
        if (throwAwayAmount == selectedItemBox.Amount)
            return;

        throwAwayAmount++;
        throwAwayAmountText.text = throwAwayAmount.ToString();
    }

    public void DecreaseThrowAwayAmount()
    {
        if (throwAwayAmount == 0)
            return;

        throwAwayAmount--;
        throwAwayAmountText.text = throwAwayAmount.ToString();
    }

    public void DecideThrowAway()
    {
        selectedItemBox.DecreaseItem(throwAwayAmount);
        selectedItemBox = null;
        explain.text = null;
        throwAwayUI.SetActive(false);
    }

    public void CancelThrowAway()
    {
        selectedItemBox = null;
        explain.text = null;
        throwAwayUI.SetActive(false);
    }

    public void OrganizeItem()
    {
        itemBoxTmp = selectedItemBox;
        selectedItemBox = null;
        explain.text = organizeText;
        choices.SetActive(false);
    }

    void OrganizeProcess(ItemBox itemBox)
    {
        if (itemBoxTmp == itemBox)
            return;

        if (itemBoxTmp.Content.ItemID == itemBox.Content.ItemID && itemBox.Amount != itemBox.Content.UpperLimit)
        {
            int dif = itemBox.Content.UpperLimit - itemBox.Amount;

            if (dif > itemBoxTmp.Amount)
                dif = itemBoxTmp.Amount;

            itemBox.IncreaseItem(dif);
            selectedItemBox = itemBoxTmp;
            itemBoxTmp.DecreaseItem(dif);
            return;

        }

        itemBoxTmp.SwapItem(itemBox);
    }

    public void DoBeforeInactive()
    {
        choices.SetActive(false);
        throwAwayUI.SetActive(false);
        selectedItemBox = null;
        itemBoxTmp = null;
        explain.text = null;
    }
}
