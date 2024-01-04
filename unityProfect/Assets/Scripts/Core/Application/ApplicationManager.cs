using UnityEngine;
using UnityEngine.EventSystems;

public class ApplicationManager : MonoBehaviour
{
    UIMediator mUIMediator;
    
    [SerializeField]
    private AppMode appMode;
    
    private void Award()
    {
        AppLaunch();
    }

    void AppLaunch()
    {
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        // 初始化管理器
        UIStateManager.Instance.Init();
        
        // 进入游戏状态
        
        // UIStateManager.Instance.EnterStatus<>();
    }
}

public enum AppMode
{
    Developing,
    QA,
    Release
}