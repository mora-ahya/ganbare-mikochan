using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    static Menu MenuInstance;
    public static Menu Instance => MenuInstance;

    [SerializeField] StrengtheningMikochan strengtheningMikochan = default;
    [SerializeField] PossessionItem possessionItem = default;
    [SerializeField] Button[] changeUIButtons = default;
    [SerializeField] Image backScreen = default;

    Color colorTmp;

    bool isOperational = true;

    public bool IsOperational
    {
        get
        {
            return isOperational;
        }

        set
        {
            isOperational = value;
        }
    }

    void Awake()
    {
        MenuInstance = this;
        possessionItem.Initialize();
        possessionItem.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void DisplayMenu()
    {
        if (!isOperational)
            return;

        if (GameSystem.Instance.Stop)
        {
            if (possessionItem.gameObject.activeSelf)
                possessionItem.DoBeforeInactive();

            if (strengtheningMikochan.gameObject.activeSelf)
                strengtheningMikochan.DoBeforeInactive();

            CameraManager.Instance.MainPostEffect.SetEffectActive(MyPostEffects.GAUSSIANBLUR_EFFECT, false);
            GameSystem.Instance.GameRestart();
            gameObject.SetActive(false);
            return;
        }

        CameraManager.Instance.MainPostEffect.SetEffectActive(MyPostEffects.GAUSSIANBLUR_EFFECT, true);
        GameSystem.Instance.GameStop();
        strengtheningMikochan.SetPossessionExpText();
        gameObject.SetActive(true);

    }

    public void ChangeUI(int state)
    {
        if (state == 0)
            possessionItem.DoBeforeInactive();

        if (state == 1)
            strengtheningMikochan.DoBeforeInactive();

        strengtheningMikochan.gameObject.SetActive(state == 0);
        possessionItem.gameObject.SetActive(state == 1);
        colorTmp = changeUIButtons[state].colors.normalColor;
        colorTmp.a = 175f / 255f;
        backScreen.color = colorTmp;

    }

    public void StoreItem(Item item, int amount)
    {
        possessionItem.StoreItem(item, amount);
    }

    public void SetSelectedItemBox(ItemBox itemBox)
    {
        possessionItem.SetSelectedItemBox(itemBox);
    }

    public void ShiftItemBoxes(ItemBox i)
    {
        possessionItem.ShiftItemBoxes(i);
    }

    public void Act()
    {

    }
}
