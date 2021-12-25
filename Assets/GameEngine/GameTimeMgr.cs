using System;
using UnityEngine;

public static class GameTimeMgr
{
    public static long DeltaTime = 0;

    public static void SetLoginTime(long loginTime)
    {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        DeltaTime = loginTime - now;
        Debug.Log(now + " " + loginTime);
    }

    public static long GetCurrentTime()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds() + DeltaTime;
    }
}
