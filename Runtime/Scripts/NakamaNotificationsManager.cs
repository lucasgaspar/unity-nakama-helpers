using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nakama.Helpers
{
    public partial class NakamaNotificationsManager : MonoBehaviour
    {
        #region FIELDS

        private const string SendNotificationRpc = "SendNotificationRpc";
        private const int NotificationsToRequest = 100;
        private const int NotificationsToShow = 50;
        private const string SaveKey = "Notification.LastSeenTime";

        [SerializeField] private List<Code> notificationsTypesToShow = new List<Code>();

        private NakamaManager nakamaManager = null;
        private List<IApiNotification> notifications = new List<IApiNotification>();

        #endregion

        #region EVENTS

        public static NakamaNotificationsManager Instance { get; private set; } = null;
        public event Action<int> onNewNotificationAmountChanged = null;
        public event Action onLoadedPendingNotifications = null;
        private Dictionary<Code, Action<IApiNotification>> onReceiveNotification = new Dictionary<Code, Action<IApiNotification>>();

        #endregion

        #region PROPERTIES

        public int NewNotificationsAmount { get; private set; } = default(int);

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            nakamaManager = NakamaManager.Instance;
            nakamaManager.onLoginSuccess += RegisterForNotifications;
            nakamaManager.onDisconnected += UnregisterForNotifications;
        }

        private void OnDestroy()
        {
            nakamaManager.onLoginSuccess -= RegisterForNotifications;
            nakamaManager.onDisconnected -= UnregisterForNotifications;
        }

        private void RegisterForNotifications()
        {
            GetPendingNotifications();
        }

        private void UnregisterForNotifications()
        {
            nakamaManager.Socket.ReceivedNotification -= ReceivedNotification;
        }

        public void Subscribe(Code code, Action<IApiNotification> action)
        {
            if (!onReceiveNotification.ContainsKey(code))
                onReceiveNotification.Add(code, null);

            onReceiveNotification[code] += action;
        }

        public void Unsubscribe(Code code, Action<IApiNotification> action)
        {
            if (onReceiveNotification.ContainsKey(code))
                onReceiveNotification[code] -= action;
        }

        private void ReceivedNotification(IApiNotification notification)
        {
            if (notificationsTypesToShow.Contains((Code)notification.Code))
                notifications.Add(notification);
            else
                DeleteNotifications(new List<string>() { notification.Id });

            if (onReceiveNotification.ContainsKey((Code)notification.Code))
                onReceiveNotification[(Code)notification.Code]?.Invoke(notification);
        }

        public async void SendNotification(Code code, string to, string subject = "", string body = "", Dictionary<string, string> data = null, bool persistent = false, bool nakama = true, bool firebase = false)
        {
            NotificationContent notificationContent = new NotificationContent((int)code, nakamaManager.Session.UserId, to, subject, body, data, persistent, nakama, firebase);
            var result = await nakamaManager.SendRPC(SendNotificationRpc, notificationContent.Serialize());
        }

        private async void GetPendingNotifications()
        {
            IApiNotificationList result = await nakamaManager.Client.ListNotificationsAsync(nakamaManager.Session, NotificationsToRequest);
            notifications.Clear();
            notifications = result.Notifications.ToList();
            List<string> notificationsToDelete = new List<string>();
            for (int i = notifications.Count - 1; i >= default(int); i--)
            {
                if (notificationsTypesToShow.Contains((Code)notifications[i].Code))
                    continue;

                notificationsToDelete.Add(notifications[i].Id);
                notifications.RemoveAt(i);
            }

            while (notifications.Count > NotificationsToShow)
            {
                notificationsToDelete.Add(notifications.First().Id);
                notifications.RemoveAt(0);
            }

            DeleteNotifications(notificationsToDelete);
            nakamaManager.Socket.ReceivedNotification += ReceivedNotification;
            onLoadedPendingNotifications?.Invoke();
            NewNotificationsAmount = GetNewNotificationsCount();
            onNewNotificationAmountChanged?.Invoke(NewNotificationsAmount);
        }

        private async void DeleteNotifications(List<string> notificationsToDelete)
        {
            await nakamaManager.Client.DeleteNotificationsAsync(nakamaManager.Session, notificationsToDelete);
        }

        public void SaveLastSeenTime()
        {
            NewNotificationsAmount = default(int);
            onNewNotificationAmountChanged?.Invoke(NewNotificationsAmount);
            PlayerPrefs.SetString(SaveKey, DateTime.UtcNow.Ticks.ToString());
        }

        public long GetLastSeenTime()
        {
            return long.Parse(PlayerPrefs.GetString(SaveKey, default(long).ToString()));
        }

        public int GetNewNotificationsCount(params Code[] codes)
        {
            int count = 0;
            foreach (IApiNotification notification in notifications)
                if (codes.Count() == 0 || codes.Contains((Code)notification.Code))
                    if (DateTime.Parse(notification.CreateTime, null, System.Globalization.DateTimeStyles.RoundtripKind).Ticks > GetLastSeenTime())
                        count++;

            return count;
        }

        public List<IApiNotification> GetNotifications(params Code[] codes)
        {
            List<IApiNotification> filteredNotifications = new List<IApiNotification>();
            foreach (IApiNotification notification in notifications)
                if (codes.Count() == 0 || codes.Contains((Code)notification.Code))
                    filteredNotifications.Add(notification);

            filteredNotifications.Reverse();
            return filteredNotifications;
        }

        #endregion
    }
}
