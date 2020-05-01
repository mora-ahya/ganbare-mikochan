using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBase : MonoBehaviour
{
    protected FunctionalStateMachine eventPhase;

    public virtual void Act()
    {
        eventPhase();
    }

    void Update()
    {
        eventPhase?.Invoke();
    }
}
