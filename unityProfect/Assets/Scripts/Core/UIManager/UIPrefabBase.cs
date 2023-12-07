using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrefab : MonoBehaviour
{
    UIPrefab parentPrefab;
    List<UIPrefab> childsPrefabs;


    #region 消息传递

    public void SendUpdateUI(string strID)
    {
        foreach (UIPrefab uIPrefab in childsPrefabs)
        {
            bool isStopTransmit = uIPrefab.OnUpdateUI(strID);
            if (isStopTransmit)
                break;
        }

        // prefab.LoadAsset
    }

    public void SendUpUI()
    {
        // parentPrefab.
    }

    /// <returns>是否阻止继续传递</returns>
    public bool OnUpdateUI(string strID,params object[] args)
    {
        return false;
    }



    #endregion
}