using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaManager : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private NakamaConnectionData connectionData = null;

        private IClient client = null;
        private ISession session = null;
        private ISocket socket = null;

        #endregion

        #region EVENTS

        public event Action onConnecting = null;
        public event Action onConnected = null;
        public event Action onDisconnected = null;
        public event Action onLoginSuccess = null;
        public event Action onLoginFail = null;

        #endregion

        #region PROPERTIES

        public static NakamaManager Instance { get; private set; } = null;
        public string UserId { get => session == null ? string.Empty : session.UserId; }
        public string Username { get => session == null ? string.Empty : session.Username; }
        public bool IsLoggedIn { get => socket != null && socket.IsConnected; }
        public ISocket Socket { get => socket; }
        public ISession Session { get => session; }
        public IClient Client { get => client; }

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            if (socket != null)
                socket.CloseAsync();
        }

        public async void LinkFacebook(string accessToken)
        {
            await client.LinkFacebookAsync(session, accessToken, true);
        }

        public void LoginWithGoogle(string token)
        {
            client = new Client(connectionData.Scheme, connectionData.Host, connectionData.Port, connectionData.ServerKey);
            LoginAsync(client.AuthenticateGoogleAsync(token));
        }

        public void LoginWithDevice()
        {
            client = new Client(connectionData.Scheme, connectionData.Host, connectionData.Port, connectionData.ServerKey);
            LoginAsync(client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier));
        }

        public void LoginWithApple(string accessToken)
        {
            client = new Client(connectionData.Scheme, connectionData.Host, connectionData.Port, connectionData.ServerKey);
            LoginAsync(client.AuthenticateAppleAsync(accessToken));
        }

        public void LoginWithCustomId(string customId)
        {
            client = new Client(connectionData.Scheme, connectionData.Host, connectionData.Port, connectionData.ServerKey);
            LoginAsync(client.AuthenticateCustomAsync(customId));
        }

        private async void LoginAsync(Task<ISession> sessionTask)
        {
            onConnecting?.Invoke();
            try
            {
                session = await sessionTask;
                Debug.Log(session);
                socket = client.NewSocket();
                socket.Connected += Connected;
                socket.Closed += Disconnected;
                await socket.ConnectAsync(session);
                onLoginSuccess?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.Log(exception);
                onLoginFail?.Invoke();
            }
        }

        public void LogOut()
        {
            socket.CloseAsync();
        }

        private void Connected()
        {
            onConnected?.Invoke();
        }

        private void Disconnected()
        {
            onDisconnected?.Invoke();
        }

        public async Task<IApiRpc> SendRPC(string rpc, string payload = "{}")
        {
            if (client == null || session == null)
                return null;

            return await client.RpcAsync(session, rpc, payload);
        }

        #endregion
    }
}
