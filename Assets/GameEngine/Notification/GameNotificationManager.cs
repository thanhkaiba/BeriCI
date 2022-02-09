using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Notifications.Android;
using UnityEngine;

namespace Piratera.Notification
{
    public class GameNotificationManager : MonoBehaviour
    {
        private static string CHANNEL_ID = "piratera_channel_0";

        void Awake()
        {
#if UNITY_ANDROID
            Debug.Log("Da dang ky");
            AndroidNotificationChannel channel = new()
            {
                Id = CHANNEL_ID,
                Name = "Default Channel",
                Importance = Importance.Default,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
            DontDestroyOnLoad(gameObject);
            SendGameNotification();
#endif
        }
        
        public static void SendGameNotification()
        {
#if UNITY_ANDROID
            Debug.Log("da Send noti");
            var notification = new AndroidNotification
            {
                Title = "Your Stamina was restored",
                Text = "3/15",
                FireTime = DateTime.Now.AddMinutes(1)
            };

            AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);
#endif


        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                SendGameNotification();
            } else
            {
                AndroidNotificationCenter.CancelAllScheduledNotifications();
            }
        }
    }
}
