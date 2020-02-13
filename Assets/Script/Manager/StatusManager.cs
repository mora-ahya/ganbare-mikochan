using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    [SerializeField] Slider hpBar;
    [SerializeField] Text hpText;
    [SerializeField] Text expText;
    [SerializeField] Mikochan miko;

    // Start is called before the first frame update
    void Start()
    {
        hpBar.maxValue = miko.MaxHP;
        hpBar.value = miko.HP;
        hpText.text = hpBar.value.ToString() + " / " + hpBar.maxValue.ToString();
    }

    public void updateHP()
    {
        hpBar.maxValue = miko.MaxHP;
        hpBar.value = miko.HP;
        hpText.text = hpBar.value.ToString() + " / " + hpBar.maxValue.ToString();
    }

    public void updateExp()
    {
        expText.text = miko.Exp.ToString();
    }
}
