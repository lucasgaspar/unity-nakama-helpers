using UnityEngine;
using UnityEngine.Events;

namespace Nakama.Helpers
{
    public class NakamaEvents : MonoBehaviour
    {
        #region FIELDS

        private NakamaManager nakamaManager = null;
        private NakamaStorageManager nakamaStorageManager = null;
        private NakamaUserManager nakamaUserManager = null;

        #endregion

        #region EVENTS

        public UnityEvent onConnecting = null;
        public UnityEvent onConnected = null;
        public UnityEvent onDisconnected = null;
        public UnityEvent onLoginSuccess = null;
        public UnityEvent onLoginFail = null;
        public UnityEvent onLoadedData = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            nakamaManager = NakamaManager.Instance;
            nakamaStorageManager = NakamaStorageManager.Instance;
            nakamaUserManager = NakamaUserManager.Instance;
            nakamaManager.onConnecting += OnConnecting;
            nakamaManager.onConnected += OnConnected;
            nakamaManager.onDisconnected += OnDisconnected;
            nakamaManager.onLoginSuccess += OnLoginSuccess;
            nakamaManager.onLoginFail += OnLoginFail;
            nakamaStorageManager.onLoaded += OnLoadedStorage;
            nakamaUserManager.onLoaded += OnLoadedUser;
        }

        private void OnDestroy()
        {
            nakamaManager.onConnecting -= OnConnecting;
            nakamaManager.onConnected -= OnConnected;
            nakamaManager.onDisconnected -= OnDisconnected;
            nakamaManager.onLoginSuccess -= OnLoginSuccess;
            nakamaManager.onLoginFail -= OnLoginFail;
            nakamaStorageManager.onLoaded -= OnLoadedStorage;
            nakamaUserManager.onLoaded -= OnLoadedUser;
        }

        private void OnConnecting()
        {
            onConnecting?.Invoke();
        }

        private void OnConnected()
        {
            onConnected?.Invoke();
        }

        private void OnDisconnected()
        {
            onDisconnected?.Invoke();
        }

        private void OnLoginSuccess()
        {
            onLoginSuccess?.Invoke();
        }

        private void OnLoginFail()
        {
            onLoginFail?.Invoke();
        }

        private void OnLoadedStorage()
        {
            if (nakamaUserManager.LoadingFinished)
                OnLoadedData();
        }

        private void OnLoadedUser()
        {
            if (nakamaStorageManager.LoadingFinished)
                OnLoadedData();
        }

        private void OnLoadedData()
        {
            onLoadedData?.Invoke();
        }

        #endregion
    }
}
