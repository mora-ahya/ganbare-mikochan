using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manju : Item
{
    void Start()
    {
        itemName = "ひとくちまんじゅう";
        explain = "ひとくちサイズで食べやすい\nHPが5回復する。";
        itemID = 1;
        upperLimit = 9;
        duration = 60;
    }

    public override void Effect()
    {
        Mikochan.Instance.ChangeHp(5);
    }
}
