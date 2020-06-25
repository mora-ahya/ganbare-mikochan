using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected string itemName;
    protected string explain;
    protected int itemID;
    protected int upperLimit;
    protected int duration;

    public string ItemName => itemName;
    public string Explain => explain;
    public int ItemID => itemID;
    public int UpperLimit => upperLimit;
    public int Duration => duration;
    public Sprite ItemSprite => itemSprite;

    [SerializeField] protected Sprite itemSprite = default;

    public abstract void Effect();
}
