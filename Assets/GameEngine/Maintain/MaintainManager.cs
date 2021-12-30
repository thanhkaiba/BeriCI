using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piratera.Engine
{
    public static class MaintainManager
    {
        private static long startTime = 0;
        private static long endTime = 0;
        private static string message = "";

        public static void OnReceiveMaintainInfo(long start, long end, string m)
        {
            startTime = start;
            endTime = end;
            message = m;

            GameEvent.MaintainDataChanged.Invoke();
        }

        public static long GetRemainTimeToMaintain()
        {
            if (startTime < GameTimeMgr.GetCurrentTime())
            {
                return 0;
            }
            return startTime - GameTimeMgr.GetCurrentTime();
        }

       public static void ResetData()
       {
            startTime = 0;
            endTime = 0;
            message = "";

            GameEvent.MaintainDataChanged.Invoke();
       }

        public static string GetMaintainMessage()
        {
            if (GetRemainTimeToMaintain() > 0)
            {
                TimeSpan remaining = TimeSpan.FromMilliseconds(GetRemainTimeToMaintain());
                return string.Format(" Server will be maintained after {0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
            } 
            else
            {
                return "";
            }
           
        }

        public static bool CanPlay()
        {
            return GetRemainTimeToMaintain() <= 0 && GameTimeMgr.GetCurrentTime() > endTime;
        }
    }
}
