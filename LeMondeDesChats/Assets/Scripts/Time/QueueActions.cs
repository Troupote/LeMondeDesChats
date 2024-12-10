using System;
using System.Collections.Generic;
using UnityEngine;

public class QueueActions : MonoBehaviour
{
    public static QueueActions QueueInstance;
    private static List<Action> queuedActions = new List<Action>();

    

    public static void ClearActions()
    {
        queuedActions.Clear();
    }

    public static void AddActions(Action action)
    {
        queuedActions.Add(action);
    }

    public static void StartActions()
    {
        foreach (var elem in queuedActions)
        {
            elem?.Invoke();
        }
    }



}