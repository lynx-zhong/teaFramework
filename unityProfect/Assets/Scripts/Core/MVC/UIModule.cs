public class UIModule
{
    protected UIMediator uiMediator;
    
    
    
    protected virtual void Init(){}
    public void OnInit()
    {
        Init();
    }
    
    protected virtual void UnInit(){}
    public void OnUnInit()
    {
        UnInit();
    }

    protected void AddMessageHandler()
    {
        
    }

    public void OnAction(string strID,params object[] args)
    {
        
    }
}