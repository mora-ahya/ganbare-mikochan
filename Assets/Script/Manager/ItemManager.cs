using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    static ItemManager ItemManagerInstance;
    public static ItemManager Instance => ItemManagerInstance;

    [SerializeField]Item[] items = default;

    void Awake()
    {
        ItemManagerInstance = this;
    }

    public Item GetItem(int itemID)
    {
        return items[itemID];
    } 
}
