using UnityEngine;

public static class Logger
{
    public static void Log(string message)
    {
        Debug.Log("[Logger] " + message);
    }
}