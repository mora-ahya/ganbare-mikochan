using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossEvent : EventBase
{
    [SerializeField] GameObject invisibleWall = default;
    [SerializeField] GameObject destination = default;
    [SerializeField] BoxCollider2D bc = default;
    [SerializeField] Gashadokuro gasha = default;

    readonly string textName = "Text/Graveyard/BossEvent";

    List<string> text = new List<string>();

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("mikochan"))
        {
            
            TextLoader.Instance.LoadText(textName, text);
            EventTextManager.Instance.Set(text);
            Mikochan.Instance.Restart();
            Mikochan.Instance.ChangeAuto();
            Mikochan.Instance.RunCommand(1);
            bc.enabled = false;
            GameSystem.Instance.Whiteness.gameObject.SetActive(false);
            eventPhase = MoveCenter;
            GameSystem.Instance.StartEvent(this);
        }
        
    }

    void MoveCenter()
    {
        if (Mikochan.Instance.transform.position.x - destination.transform.position.x < 0)
            return;

        Mikochan.Instance.StopCommand();
        invisibleWall.SetActive(true);
        EventTextManager.Instance.gameObject.SetActive(true);
        CameraManager.Instance.FixedCamera(true);
        eventPhase = WaitText;
    }

    void WaitText()
    {
        if (EventTextManager.Instance.gameObject.activeSelf)
            return;

        gasha.gameObject.SetActive(true);
        eventPhase = GashaAppearance;
    }

    void GashaAppearance()
    {
        if (!gasha.AppearanceProcess())
            return;

        eventPhase = StartToBattle;
        EventTextManager.Instance.gameObject.SetActive(true);
    }

    void StartToBattle()
    {
        if (EventTextManager.Instance.gameObject.activeSelf)
            return;

        Mikochan.Instance.SetActiveQuickUse(true);
        Mikochan.Instance.ChangeOperational();
        Menu.Instance.IsOperational = false;
        gasha.StartToBattle();
        eventPhase = null;
    }

    public void BeatenBoss()
    {
        StartCoroutine(Beaten());
        GameSystem.Instance.Whiteness.gameObject.SetActive(true);
        GameSystem.Instance.Whiteness.CrossFadeAlpha(0f, 0.2f, false);
    }

    IEnumerator Beaten()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 1.0f;
        yield return GameSystem.Instance.FourSecond;
        GameSystem.Instance.GameClear();
    }
}
