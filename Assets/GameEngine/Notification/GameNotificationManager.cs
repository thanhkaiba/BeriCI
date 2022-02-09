using Piratera.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Notifications.Android;
using UnityEngine;

namespace Piratera.Notification
{
    public class GameNotificationRequest
    {
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
                Name = "Default Channel",
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

            AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);
#endif


        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                AndroidNotificationCenter.CancelAllScheduledNotifications();
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
                long remainTime = StaminaData.Instance.TimeToHaveNewStamina();
                int curStamina = StaminaData.Instance.Stamina;
                int maxStamina = GlobalConfigs.StaminaConfig.max_stamina;
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
                            Debug.Log("Da dang ky stamina notification: " + $"{i}/{maxStamina}" + " after " + ((i - curStamina - 1) * countdownTime + remainTime));
                            SendGameNotification(new GameNotificationRequest
                            {
                                Title = "🔋 Your Stamina was restored! ⚡⚡⚡",
                                Content = $"{i}/{maxStamina} ⚡",
                                FireTime = DateTime.Now.AddSeconds((i - curStamina - 1) * countdownTime + remainTime)
                            });
                        }

                    }
                }
            }
         
        }
    }
}
