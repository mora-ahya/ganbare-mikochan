using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBase : MonoBehaviour
{
    protected FunctionalStateMachine eventPhase;

    public void Act()
    {
        eventPhase?.Invoke();
    }
}
