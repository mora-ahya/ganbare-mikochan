using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    static ItemManager ItemManagerInstance;
    public static ItemManager Instance => ItemManagerInstance;

    public readonly int PurifiedWaterID = 0;

    [SerializeField]Item[] items = default;

    void Awake()
    {
        ItemManagerInstance = this;
    }

    public Item GetItemInfo(int itemID)
    {
        return items[itemID];
    } 
}
