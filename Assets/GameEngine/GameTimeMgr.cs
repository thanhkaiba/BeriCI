using System;

public static class GameTimeMgr
{
    public static long DeltaTime = 0;

    public static void SetLoginTime(long loginTime)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        DeltaTime = now - loginTime;
    }

    public static long GetCurrentTime()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + DeltaTime;
    }
}
