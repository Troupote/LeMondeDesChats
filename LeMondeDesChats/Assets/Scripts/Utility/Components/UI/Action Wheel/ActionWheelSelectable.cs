using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class ActionWheelSelectable : Selectable
{
    [Serializable]
    public class TriggerEvent : UnityEvent { }

    public TriggerEvent OnTriggerEvent = new TriggerEvent();

    public void InvokeTrigger() => OnTriggerEvent.Invoke();

}
