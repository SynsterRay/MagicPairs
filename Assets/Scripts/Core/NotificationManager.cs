using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

namespace MagicPairs.Core
{
    public class NotificationManager : MonoBehaviour
    {
        private const string ChannelId = "magicpairs_default";

        private void Start()
        {
#if UNITY_ANDROID
            CreateChannel();
#endif
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
                ScheduleNotifications();
            else
                CancelAll();
        }

#if UNITY_ANDROID
        private void CreateChannel()
        {
            var channel = new AndroidNotificationChannel
            {
                Id = ChannelId,
                Name = "Magic Pairs",
                Importance = Importance.Default,
                Description = "Game reminders"
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        private void ScheduleNotifications()
        {
            AndroidNotificationCenter.CancelAllNotifications();

            // Daily bonus reminder — 24h from now
            SendNotification("Your daily bonus is waiting! 🎁",
                "Come back and claim your free reward!", 24);

            // Comeback reminder — 48h from now
            SendNotification("We miss you! 🃏",
                "Your cards are waiting. Can you beat your high score?", 48);

            // Weekly challenge — 72h
            SendNotification("Challenge yourself! 🏆",
                "How far can you get in Challenge mode?", 72);
        }

        private void SendNotification(string title, string text, int hoursFromNow)
        {
            var notification = new AndroidNotification
            {
                Title = title,
                Text = text,
                FireTime = System.DateTime.Now.AddHours(hoursFromNow),
                SmallIcon = "icon_0",
                LargeIcon = "icon_1"
            };
            AndroidNotificationCenter.SendNotification(notification, ChannelId);
        }

        private void CancelAll()
        {
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
        }
#else
        private void ScheduleNotifications() { }
        private void CancelAll() { }
#endif
    }
}
