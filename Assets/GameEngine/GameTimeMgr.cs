using System;
using UnityEngine;

public static class GameTimeMgr
{
    public static long DeltaTime = 0;

    private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    /// <summary>Get extra long current timestamp</summary>
    public static long Millis { get { return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds); } }

    public static void SetLoginTime(long loginTime)
    {
        DeltaTime = Millis - loginTime;

        Debug.Log("Login Time " + loginTime + " " + DeltaTime + " now: " + Millis);
    }

    public static long GetCurrentTime()
    {
        return Millis + DeltaTime;
    }
}
