using Piratera.Config;
using Piratera.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

namespace Piratera.Notification
{
    public class GameNotificationRequest
    {
        public int Id;
        public string Title;
        public string Content;
        public DateTime FireTime;
    }
    public class GameNotificationManager : MonoBehaviour
    {
        private static string CHANNEL_ID = "piratera_channel_0";
        private static readonly List<GameNotificationRequest> requests = new();

        void Awake()
        {
#if UNITY_ANDROID
            AndroidNotificationChannel channel = new()
            {
                Id = CHANNEL_ID,
                Name = "Dead men tell no tales",
                Importance = Importance.Default,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
            DontDestroyOnLoad(gameObject);
#endif
        }
        
        private static void SendGameNotification(GameNotificationRequest request)
        {
#if UNITY_ANDROID
            var notification = new AndroidNotification
            {
                Title = request.Title,
                Text = request.Content,
                FireTime = request.FireTime,
            };


            var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(request.Id);
            Debug.Log("Notification Status: " + notificationStatus);

            if (notificationStatus == NotificationStatus.Scheduled)
            {
                Debug.Log("da update notification");
                // Replace the scheduled notification with a new notification.
                AndroidNotificationCenter.UpdateScheduledNotification(request.Id, notification, CHANNEL_ID);
            }
            else 
            {
                AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);
            }
          
#endif


        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus && NetworkController.IsInitialized)
            {
#if UNITY_ANDROID
                Debug.Log("da remove all notification");
                AndroidNotificationCenter.CancelAllScheduledNotifications();
#endif
            } else
            {
                RegisterStaminaNotification();
                foreach (var request in requests)
                {
                    SendGameNotification(request);
                }
            }
        }

        private void RegisterStaminaNotification()
        {
          
            if (GlobalConfigs.StaminaConfig != null)
            {
                long remainTime = StaminaData.Instance.TimeToHaveNewStamina() / 1000;
                int curStamina = StaminaData.Instance.Stamina;
                int maxStamina = GlobalConfigs.StaminaConfig.GetStaminaCapacity();
                int countdownTime = GlobalConfigs.StaminaConfig.recovering_time;
                if (remainTime > 0 || curStamina < maxStamina)
                {
                    if (remainTime < 0)
                    {
                        remainTime = 0;
                    }
                    for (int i = curStamina + 1; i <= maxStamina; i++)
                    {
                        if (i % 3 == 0)
                        {
                            long afterTime = ((i - curStamina - 1) * countdownTime + remainTime);
                           // Debug.Log("Da dang ky stamina notification: " + $"{i}/{maxStamina}" + " after " + afterTime);
                            SendGameNotification(new GameNotificationRequest
                            {
                                Title = "🔋Yo-ho-ho! Your Stamina was restored!⚡⚡⚡",
                                Content = $"{i}/{maxStamina} ⚡",
                                FireTime = DateTime.Now.AddSeconds(afterTime),
                                Id = 30 + i,
                            });
                        }

                    }

                }
            }
         
        }
    }
}
