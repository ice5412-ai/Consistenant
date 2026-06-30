using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

namespace Habillage
{
    public class AndroidNotificationControl : MonoBehaviour
    {
#if UNITY_ANDROID
        // Request authorization to send notification
        public void RequestAuthorization()
        {

            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }

        }

        // Register a notification channel
        public void RegisterNotificationChannel()
        {
            var channel = new AndroidNotificationChannel
            {
                Id = "default_channel",
                Name = "Default_Channel",
                Importance = Importance.Default,
                Description = "Default Notification"
            };

            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        // Set up notification template

        public void SendNotification(string title, string text, DateTime time, IconListEnum SmallIcon, IconListEnum LargeIcon, int notificationId)
        {
            if (time.AddSeconds(10) < DateTime.Now)
            {
                return;
            }

            Debug.Log("Notification set : " + title + " : " + text + " at " + time);

            var notification = new AndroidNotification();
            notification.Title = title; // Headline
            notification.Text = text; // Description
            notification.FireTime = time;
            notification.SmallIcon = IconList.FromEnum(SmallIcon);
            notification.LargeIcon = IconList.FromEnum(LargeIcon);
            notification.Color = new Color(0.9098039f, 0.7686275f, 0.4196078f);

            // Cancel any existing notification with the same ID
            AndroidNotificationCenter.CancelNotification(notificationId);

            // Send the new notification with the specified ID
            AndroidNotificationCenter.SendNotification(notification, "default_channel");
        }

        public void CancelNotification(int notificationId)
        {
            AndroidNotificationCenter.CancelNotification(notificationId);
        }
#endif
    }
}