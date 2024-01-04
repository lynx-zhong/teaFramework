using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMediator
{
    private List<UIPrefab> childPrefabs;
    private List<UIPrefab> childAsynPrefabs;
    private UIModule module;
    
    protected virtual void ApplicationStatusEnter(params object[] args) { }
    public void OnApplicationStatusEnter(params object[] args)
    {
        childPrefabs = new List<UIPrefab>();
        childAsynPrefabs = new List<UIPrefab>();
        
        ApplicationStatusEnter(args);
    }

    protected virtual void ApplicationStatusExit(params object[] args) { }
    public void OnApplicationStatusExit(params object[] args)
    {
        ApplicationStatusExit(args);
    }
    
    public UIPrefab CreatePrefab<T>(params object[] args) where T : UIPrefab
    {
        UIPrefab uiPrefab = Activator.CreateInstance<T>();
        childPrefabs.Add(uiPrefab);
        return uiPrefab;
    }

    public void CreateAsynPrefab<T>() where T : UIPrefab
    {
        
    }

    protected void SendAction(string strID, params object[] args)
    {
        module.OnAction(strID,args);
    }

    protected void OnAction(string strID, params object[] args)
    {

    }
    
    protected void SendUpdateUI(string strID, params object[] args)
    {
        foreach (UIPrefab uiPrefab in childPrefabs)
        {
            if (uiPrefab.OnUpdateUI(strID, args))
                return;
        }

        foreach (UIPrefab uiPrefab in childAsynPrefabs)
        {
            if (uiPrefab.OnUpdateUI(strID, args))
                return;
        }
    }

    protected void OnUpdateUI(string strID, params object[] args)
    {

    }
}
