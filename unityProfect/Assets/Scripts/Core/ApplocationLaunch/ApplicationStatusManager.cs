using System;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationStatusManager : Singleton<ApplicationStatusManager>
{
    Stack<IApplicationStatus> statusStack = new Stack<IApplicationStatus>();

    IApplicationStatus rootStatus;

    public void EnterStatus<T>(params object[] newStatusparamArray) where T : IApplicationStatus
    {
        IApplicationStatus newStatus = Activator.CreateInstance<T>();
        statusStack.Push(newStatus);
        newStatus.OnApplocationStatusEnter(newStatusparamArray);
    }

    public void PopStatus()
    {
        if (statusStack.Count <= 1)
        {
            Logger.LogError("in root status, can not pop status");
            return;
        }

        IApplicationStatus oldStatus = statusStack.Pop();
        if (oldStatus != null)
        {
            oldStatus.ApplicationStatusExit();
        }


    }
}