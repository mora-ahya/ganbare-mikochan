using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickUseItem : MonoBehaviour
{
    [SerializeField] PossessionItem possessionItem = default;
    [SerializeField] Image itemImage = default;
    [SerializeField] Text itemText = default;
    [SerializeField] Sprite transparent = default;
    [SerializeField] Transition transition = default;

    int selectNum = 0;
    ItemBox selectedItemBox;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Act()
    {
        GetInput();
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            QuickUse();
            return;
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            Shift(false);
            return;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            Shift(true);
            return;
        }
    }

    public void SetItemInfo()
    {
        selectNum = 0;
        if (possessionItem.ItemAmount == 0)
        {
            selectedItemBox = null;
            itemText.text = null;
            return;
        }

        selectedItemBox = possessionItem.GetItemBoxInfo(selectNum);
        itemImage.sprite = selectedItemBox.Content.ItemSprite;
        itemText.text = selectedItemBox.Amount.ToString();
    }

    public void Shift(bool toNext)
    {
        int tmp = possessionItem.ItemAmount;
        if (tmp == 0)
            return;

        selectNum += (toNext ? 1 : -1);

        if (selectNum < 0)
            selectNum += tmp;        

        if (selectNum == tmp)
            selectNum = 0;

        selectedItemBox = possessionItem.GetItemBoxInfo(selectNum);
        itemImage.sprite = selectedItemBox.Content.ItemSprite;
        itemText.text = selectedItemBox.Amount.ToString();

    }

    public void QuickUse()
    {
        if (selectedItemBox == null)
            return;

        selectedItemBox.UseItem();
        transition.HealEffect();

        if (possessionItem.ItemAmount == 0)
        {
            selectedItemBox = null;
            itemImage.sprite = transparent;
            itemText.text = null;
            return;
        }

        if (selectNum != 0 && selectNum == possessionItem.ItemAmount)
            selectedItemBox = possessionItem.GetItemBoxInfo(--selectNum);

        itemImage.sprite = selectedItemBox.Content.ItemSprite;
        itemText.text = selectedItemBox.Amount.ToString();

    }
}
