using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaWalletManager : MonoBehaviour
    {
        #region FIELDS

        private const string WalletTransactionRpc = "WalletTransactionRpc";

        private NakamaManager nakamaManager = null;
        private NakamaUserManager nakamaUserManager = null;
        private NakamaNotificationsManager nakamaNotificationsManager = null;
        private string wallet = null;

        #endregion

        #region EVENTS

        public event Action onLoaded = null;
        public event Action onUpdate = null;

        #endregion

        #region PROPERTIES

        public bool LoadingFinished { get; private set; } = false;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            nakamaManager = NakamaManager.Instance;
            nakamaUserManager = NakamaUserManager.Instance;
            nakamaNotificationsManager = NakamaNotificationsManager.Instance;
            nakamaUserManager.onLoaded += LoadWallet;
            nakamaNotificationsManager.Subscribe(NakamaNotificationsManager.Code.WalletUpdated, WalletUpdatedNotificationReceived);
        }

        private void OnDestroy()
        {
            nakamaUserManager.onLoaded -= LoadWallet;
            nakamaNotificationsManager.Unsubscribe(NakamaNotificationsManager.Code.WalletUpdated, WalletUpdatedNotificationReceived);
        }

        private void LoadWallet()
        {
            wallet = nakamaUserManager.Wallet;
            LoadingFinished = true;
            onUpdate?.Invoke();
            onLoaded?.Invoke();
        }

        private void WalletUpdatedNotificationReceived(IApiNotification notification)
        {
            wallet = notification.Content;
            onUpdate?.Invoke();
        }

        public async Task<bool> WalletTransaction(string wallet)
        {
            var result = await nakamaManager.SendRPC(WalletTransactionRpc, wallet);
            return result.Payload.Deserialize<NakamaRpcResponse>().Success;
        }

        public T GetWallet<T>()
        {
            if (wallet == null)
                return default(T);

            return wallet.Deserialize<T>();
        }

        #endregion
    }
}
