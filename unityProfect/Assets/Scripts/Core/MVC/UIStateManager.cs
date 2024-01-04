using System;
using System.Collections.Generic;
using UnityEngine;

public class UIStateManager : Singleton<UIStateManager>
{
    private bool isSwitching;
    
    Stack<UIMediator> statusStack;

    UIMediator rootStatus;

    public void Init()
    {
        statusStack = new Stack<UIMediator>();
    }

    public void UnInit()
    {
        
    }

    public void EnterStatus<T>(params object[] args) where T : UIMediator
    {
        UIMediator newStatus = Activator.CreateInstance<T>();
        statusStack.Push(newStatus);
        newStatus.OnApplicationStatusEnter(args);
    }

    public void PopStatus(params object[] args)
    {
        if (statusStack.Count <= 1)
        {
            Logger.LogError("in root status, can not pop status");
            return;
        }

        UIMediator oldStatus = statusStack.Pop();
        if (oldStatus != null)
        {
            oldStatus.OnApplicationStatusExit();
        }
        
        UIMediator showStatus = statusStack.Peek();
        showStatus.OnApplicationStatusEnter(args);
    }
}