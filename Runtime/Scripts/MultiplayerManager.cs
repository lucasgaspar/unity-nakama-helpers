using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nakama.Helpers
{
    public partial class MultiplayerManager : MonoBehaviour
    {
        #region FIELDS

        private const int TickRate = 5;
        private const float SendRate = 1f / (float)TickRate;
        private const string JoinPublicMatchRpc = "JoinPublicMatchRpc";
        private const string CreatePrivateMatchRpc = "CreatePrivateMatchRpc";
        private const string LogFormat = "{0} with code {1}:\n{2}";
        private const string SendingDataLog = "Sending data";
        private const string ReceivedDataLog = "Received data";

        [SerializeField] private bool enableLog = false;

        private NakamaManager nakamaManager = null;
        private NakamaUserManager nakamaUserManager = null;
        private Dictionary<Code, Action<MultiplayerMessage>> onReceiveData = new Dictionary<Code, Action<MultiplayerMessage>>();
        private IMatch match = null;

        #endregion

        #region EVENTS

        public event Action onMatchJoin = null;
        public event Action<string> onMatchJoinFailed = null;
        public event Action onMatchLeave = null;
        public event Action onLocalTick = null;
        public event Action onOfflineMatch = null;

        #endregion

        #region PROPERTIES

        public static MultiplayerManager Instance { get; private set; } = null;
        public string MatchId { get => match != null ? match.Id : String.Empty; }
        public IUserPresence Self { get; private set; }
        public bool IsOnMatch { get => match != null; }
        public bool IsOfflineMode { get; private set; } = true;
        public bool IsPrivate { get; private set; } = true;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            nakamaManager = NakamaManager.Instance;
            nakamaUserManager = NakamaUserManager.Instance;
            InvokeRepeating(nameof(LocalTickPassed), SendRate, SendRate);
        }

        private void LocalTickPassed()
        {
            onLocalTick?.Invoke();
        }

        public void StartOfflineMatch()
        {
            IsOfflineMode = true;
            Self = new UserPresence(nakamaManager.UserId, nakamaManager.Username);
            onOfflineMatch?.Invoke();
        }

        public void JoinPublicMatch()
        {
            GetMatchId(JoinPublicMatchRpc);
        }

        public async void JoinMatchAsync(string matchId)
        {
            IsOfflineMode = false;
            nakamaManager.Socket.ReceivedMatchState -= Receive;
            nakamaManager.Socket.ReceivedMatchState += Receive;
            nakamaManager.onDisconnected += Disconnected;
            try
            {
                match = await nakamaManager.Socket.JoinMatchAsync(matchId);
            }
            catch (Exception exception)
            {
                onMatchJoinFailed?.Invoke(exception.Message);
                return;
            }

            List<IUserPresence> presences = new List<IUserPresence>(match.Presences);
            Self = match.Self;
            presences.Add(Self);
            nakamaUserManager.SetBusyStatus();
            onMatchJoin?.Invoke();
        }

        public void CreatePrivateMatch()
        {
            IsPrivate = true;
            GetMatchId(CreatePrivateMatchRpc);
        }

        private async void GetMatchId(string rpc, string payload = "{}")
        {
            IApiRpc rpcResult = await nakamaManager.SendRPC(rpc, payload);
            JoinMatchAsync(rpcResult.Payload);
        }

        private void Disconnected()
        {
            nakamaManager.onDisconnected -= Disconnected;
            nakamaManager.Socket.ReceivedMatchState -= Receive;
            match = null;
            onMatchLeave?.Invoke();
            nakamaUserManager.SetOnlineStatus();
        }

        public async void LeaveMatchAsync()
        {
            nakamaManager.onDisconnected -= Disconnected;
            nakamaManager.Socket.ReceivedMatchState -= Receive;
            await nakamaManager.Socket.LeaveMatchAsync(match);
            match = null;
            onMatchLeave?.Invoke();
            nakamaUserManager.SetOnlineStatus();
        }

        public void Send(Code code, object data = null)
        {
            if (match == null && !IsOfflineMode)
                return;

            string json = data != null ? data.Serialize() : string.Empty;
            if (enableLog)
                LogData(SendingDataLog, (long)code, json);

            if (IsOfflineMode)
                ProcessOfflineMessage(new MultiplayerMessage(code, Self.UserId, Self.SessionId, Self.Username, data));
            else
                nakamaManager.Socket.SendMatchStateAsync(match.Id, (long)code, json);
        }

        private void ProcessOfflineMessage(MultiplayerMessage multiplayerMessage)
        {
            if (onReceiveData.ContainsKey(multiplayerMessage.DataCode))
                onReceiveData[multiplayerMessage.DataCode]?.Invoke(multiplayerMessage);
        }

        private void Receive(IMatchState newState)
        {
            if (enableLog)
            {
                var encoding = System.Text.Encoding.UTF8;
                var json = encoding.GetString(newState.State);
                LogData(ReceivedDataLog, newState.OpCode, json);
            }

            MultiplayerMessage multiplayerMessage = new MultiplayerMessage(newState);
            if (onReceiveData.ContainsKey(multiplayerMessage.DataCode))
                onReceiveData[multiplayerMessage.DataCode]?.Invoke(multiplayerMessage);
        }

        public void Subscribe(Code code, Action<MultiplayerMessage> action)
        {
            if (!onReceiveData.ContainsKey(code))
                onReceiveData.Add(code, null);

            onReceiveData[code] += action;
        }

        public void Unsubscribe(Code code, Action<MultiplayerMessage> action)
        {
            if (onReceiveData.ContainsKey(code))
                onReceiveData[code] -= action;
        }

        private void LogData(string description, long dataCode, string json)
        {
            Debug.Log(string.Format(LogFormat, description, (Code)dataCode, json));
        }

        #endregion
    }
}
