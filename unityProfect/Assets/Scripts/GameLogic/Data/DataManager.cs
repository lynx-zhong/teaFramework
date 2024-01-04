public class DataManager : Singleton<DataManager>
{
    public LoginData LoginData;
    
    public void Init()
    {
        LoginData = new LoginData();
    }

    
}