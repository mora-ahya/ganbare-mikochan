using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstEvent : EventBase
{
    [SerializeField] GameObject target = default;
    readonly string textName = "Text/Graveyard/FirstEvent";

    List<string> text = new List<string>();

    void Start()
    {
        TextLoader.Instance.LoadText(textName, text);
        EventTextManager.Instance.Set(text);
        eventPhase = MoveMikochan;
        Mikochan.Instance.Restart();
        Mikochan.Instance.ChangeAuto();
        Mikochan.Instance.RunCommand(1);
        GameSystem.Instance.StartEvent(this);
    }

    void MoveMikochan()
    {
        if (Mikochan.Instance.transform.position.x - target.transform.position.x <= 0f)
            return;

        eventPhase = EndEvent;
        EventTextManager.Instance.gameObject.SetActive(true);
        Mikochan.Instance.StopCommand();
    }

    void EndEvent()
    {
        if (EventTextManager.Instance.gameObject.activeSelf)
            return;

        Mikochan.Instance.ChangeOperational();
        eventPhase = null;
    }
}
