using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    static StatusManager statusManagerInstance;
    public static StatusManager Instance => statusManagerInstance;

    public GameObject ExpImage = default;

    [SerializeField] Slider hpBar = default;
    [SerializeField] Text hpText = default;
    [SerializeField] Text expText = default;

    void Awake()
    {
        statusManagerInstance = this;
    }

    void Start()
    {
        hpBar.maxValue = Mikochan.Instance.MaxHP;
        hpBar.value = Mikochan.Instance.HP;
        hpText.text = hpBar.value.ToString() + " / " + hpBar.maxValue.ToString();
    }

    public void updateHP()
    {
        hpBar.maxValue = Mikochan.Instance.MaxHP;
        hpBar.value = Mikochan.Instance.HP;
        hpText.text = hpBar.value.ToString() + " / " + hpBar.maxValue.ToString();
    }

    public void updateExp()
    {
        expText.text = Mikochan.Instance.Exp.ToString();
    }
}
