using System;
using UnityEngine;

public static class GameTimeMgr
{
    public static long DeltaTime = 0;

    public static void SetServerTime(long loginTime)
    {
        Debug.Log("Server Time: " + loginTime);
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        DeltaTime = loginTime - now;
    }

    public static long GetCurrentTime()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds() + DeltaTime;
    }

    public static long GetCurrentUTCTime()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public static double GetTimeToNextDayUTC()
    {
        DateTime now = DateTime.UtcNow;
        DateTime tomorrow = now.AddDays(1).Date;

       return (tomorrow - now).TotalMilliseconds;
    }
}
