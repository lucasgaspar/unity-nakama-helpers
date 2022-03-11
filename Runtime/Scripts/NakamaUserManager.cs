using System;
using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaUserManager : MonoBehaviour
    {
        #region FIELDS

        private const string OnlineStatus = "Online";
        private const string BusyStatus = "Busy";
        private const string IncreaseLevelRpc = "IncreaseLevelRpc";

        private NakamaManager nakamaManager = null;
        private NakamaNotificationsManager nakamaNotificationsManager = null;
        private IApiAccount account = null;

        #endregion

        #region EVENTS

        public event Action onLoaded = null;
        public event Action<string> onMetadataUpdated = null;

        #endregion

        #region PROPERTIES

        public static NakamaUserManager Instance { get; private set; } = null;
        public bool IsNewUser { get; set; } = false;
        public bool LoadingFinished { get; private set; } = false;
        public IApiUser User { get => account.User; }
        public string Wallet { get => account.Wallet; }
        public string Metadata { get; private set; } = "";

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            nakamaManager = NakamaManager.Instance;
            nakamaNotificationsManager = NakamaNotificationsManager.Instance;
            nakamaManager.onLoginSuccess += AutoLoad;
            nakamaNotificationsManager.Subscribe(NakamaNotificationsManager.Code.MetadataUpdated, MetadataUpdated);
        }

        private void OnDestroy()
        {
            nakamaManager.onLoginSuccess -= AutoLoad;
            nakamaNotificationsManager.Unsubscribe(NakamaNotificationsManager.Code.MetadataUpdated, MetadataUpdated);
        }

        private void MetadataUpdated(IApiNotification notification)
        {
            Metadata = notification.Content;
            onMetadataUpdated?.Invoke(Metadata);
        }

        private void AutoLoad()
        {
            LoadAsync();
            SetOnlineStatus();
        }

        private async void LoadAsync()
        {
            account = await nakamaManager.Client.GetAccountAsync(nakamaManager.Session);
            Metadata = account.User.Metadata;
            onMetadataUpdated?.Invoke(Metadata);
            LoadingFinished = true;
            if (string.IsNullOrEmpty(account.User.DisplayName))
                IsNewUser = true;

            onLoaded?.Invoke();
        }

        public async void UpdateDisplayName(string displayName)
        {
            await nakamaManager.Client.UpdateAccountAsync(nakamaManager.Session, null, displayName);
            LoadAsync();
        }

        public async void UpdateProfilePicture(string profilePicture)
        {
            await nakamaManager.Client.UpdateAccountAsync(nakamaManager.Session, null, avatarUrl: profilePicture);
        }

        public async void SetOnlineStatus()
        {
            if (nakamaManager.Socket == null)
                return;

            await nakamaManager.Socket.UpdateStatusAsync(OnlineStatus);
        }

        public async void SetBusyStatus()
        {
            if (nakamaManager.Socket == null)
                return;

            await nakamaManager.Socket.UpdateStatusAsync(BusyStatus);
        }

        public async void IncreaseLevel()
        {
            if (nakamaManager.Socket == null)
                return;

            await nakamaManager.Socket.RpcAsync(IncreaseLevelRpc);
        }

        #endregion
    }
}
