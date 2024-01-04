using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrefab : MonoBehaviour
{
    private UIPrefab parentPrefab;
    
    private List<UIPrefab> childsPrefabs;

    /// <summary>
    /// 同步创建资源
    /// </summary>
    public void CreateChildPrefab<T>() where T : UIPrefab
    {
        
    }
    
    /// <summary>
    /// 异步创建资源
    /// </summary>
    public void CreateAsynChildPrefabs()
    {
        
    }
    
    /// <summary>
    /// 摧毁子物体
    /// </summary>
    public void DestroyChildPrefab(UIPrefab uiPrefab)
    {
        
    }

    /// <summary>
    /// 摧毁该prefab下的所有子物体
    /// </summary>
    public void DestroyAllChildPrefab()
    {
        
    }
    
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
}