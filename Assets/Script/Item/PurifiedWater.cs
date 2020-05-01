using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurifiedWater : Item
{
    void Start()
    {
        itemName = "清めの水";
    }

    public override void Effect()
    {
        Mikochan.Instance.ChangeHp(10);
    }
}
