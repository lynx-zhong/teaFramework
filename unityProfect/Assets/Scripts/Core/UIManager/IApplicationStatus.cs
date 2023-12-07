using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IApplicationStatus : MonoBehaviour
{
    List<UIPrefab> childsPrefabs;

    public void ApplicaitonStatusEnter(params object[] args)
    {

    }

    public void ApplicationStatusExit(params object[] args)
    {

    }

    public void OnAction(string strID, params object[] args)
    {

    }

    public void OnUpdateUI(string strID, params object[] args)
    {
        foreach (UIPrefab uIPrefab in childsPrefabs)
        {
            if (uIPrefab.OnUpdateUI(strID, args))
                break;
        }
    }

    public UIPrefab CreatePrefab<T>(params object[] args) where T : UIPrefab
    {
        UIPrefab uiprefab = Activator.CreateInstance<T>();
        childsPrefabs.Add(uiprefab);
        return null;
    }

    #region 子类
    public virtual void OnApplocationStatusEnter(params object[] args) { }

    public virtual void OnApplocationStatusExit(params object[] args) { }

    #endregion
    // public void 
}
