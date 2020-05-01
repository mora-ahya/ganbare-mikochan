﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : EventBase
{
    [SerializeField] BoxCollider2D bc = default;
    [SerializeField] Scarecrow scarecrow = default;
    readonly string textName = "Text/tutorial";

    List<string> tutorialText = new List<string>();
    int couter = 0;
    
    void Start()
    {
        TextLoader.Instance.LoadText(textName, tutorialText);
        //eventPhase = Test;
    }

    void FirstWaitText()
    {
        if (EventTextManager.Instance.gameObject.activeSelf)
            return;

        eventPhase = GetCloseScarecrow;
        Mikochan.Instance.RunCommand(1);
    }

    void GetCloseScarecrow()
    {
        if (Mathf.Abs(Mikochan.Instance.transform.position.x - scarecrow.transform.position.x) >= 1.5f)
            return;

        eventPhase = SecondWaitText;
        EventTextManager.Instance.gameObject.SetActive(true);
        Mikochan.Instance.StopCommand();
    }


    void SecondWaitText()
    {
        if (!EventTextManager.Instance.IsStayed)
            return;

        eventPhase = StunScarecrow;
        scarecrow.Restart();
    }

    void StunScarecrow()
    {
        scarecrow.Act();
        if (!scarecrow.Stun)
            return;

        eventPhase = AwokenScarecrow;
        EventTextManager.Instance.IsStayed = false;
    }

    void AwokenScarecrow()
    {
        scarecrow.Act();
        if (scarecrow.Stun || !EventTextManager.Instance.IsStayed)
            return;

        EventTextManager.Instance.IsStayed = false;
        scarecrow.Pause();
        eventPhase = ThirdWaitText;
    }

    void ThirdWaitText()
    {
        if (!EventTextManager.Instance.IsStayed)
            return;

        scarecrow.Restart();
        eventPhase = SecondStunScarecrow;
    }

    void SecondStunScarecrow()
    {
        scarecrow.Act();
        if (!scarecrow.Stun)
            return;

        EventTextManager.Instance.IsStayed = false;
        scarecrow.Pause();
        Mikochan.Instance.RunCommand(1);
        eventPhase = MikochanOnScarecrow;
    }

    void MikochanOnScarecrow()
    {
        if (Mikochan.Instance.transform.position.x < scarecrow.transform.position.x)
            return;

        Mikochan.Instance.StopCommand();
        eventPhase = FourthWaitText;
    }

    void FourthWaitText()
    {
        if (!EventTextManager.Instance.IsStayed)
            return;

        eventPhase = SealScarecrow;
    }

    void SealScarecrow()
    {
        if (!Input.GetKeyDown(KeyCode.S))
            return;

        //みこちゃん封印コマンド
        scarecrow.Restart();
        scarecrow.Sealed();
        eventPhase = EndTutorial;
        EventTextManager.Instance.IsStayed = false;
    }

    void EndTutorial()
    {
        scarecrow.Act();
        if (EventTextManager.Instance.gameObject.activeSelf)
            return;

        Mikochan.Instance.ChangeOperational();
        eventPhase = null;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Mikochan.Instance.tag))
        {
            EventTextManager.Instance.Set(tutorialText);
            eventPhase = FirstWaitText;
            Mikochan.Instance.Restart();
            Mikochan.Instance.ChangeAuto();
            Mikochan.Instance.StopCommand();
            bc.enabled = false;
        }
    }
}
