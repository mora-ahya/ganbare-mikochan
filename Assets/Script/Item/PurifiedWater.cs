using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurifiedWater : Item
{
    void Start()
    {
        itemName = "清らかな水";
        explain = "清らかで澄んだ水\nHPが20回復する。";
        itemID = 0;
        upperLimit = 5;
        duration = 180;
    }

    public override void Effect()
    {
        Mikochan.Instance.ChangeHp(20);
    }
}
