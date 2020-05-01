using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected string itemName;

    public string ItemName => itemName;
    [SerializeField] Sprite itemSprite = default;

    public abstract void Effect();
}
