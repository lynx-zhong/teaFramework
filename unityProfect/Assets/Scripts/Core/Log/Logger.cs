using UnityEngine;

public class Logger
{
    public static void Log(params object[] logs)
    {
        string logText = string.Join("\t", logs);
        Debug.Log(logText);
    }

    public static void LogError(params object[] logs)
    {
        string logText = string.Join("\t", logs);
        Debug.LogError(logText);
    }

    public static void LogWarning(params object[] logs)
    {
        string logText = string.Join("\t", logs);
        Debug.LogWarning(logText);
    }
}