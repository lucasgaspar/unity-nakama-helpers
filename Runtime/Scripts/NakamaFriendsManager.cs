using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nakama.Helpers
{
    public partial class NakamaFriendsManager : MonoBehaviour
    {
        #region FIELDS

        private const int FriendsLimit = 100;

        [SerializeField] private string debugId = "";

        private NakamaManager nakamaManager = null;
        private NakamaNotificationsManager nakamaNotificationsManager = null;

        #endregion

        #region EVENTS

        private Dictionary<string, Action<string>> onStatusChange = new Dictionary<string, Action<string>>();

        #endregion

        #region PROPERTIES

        public static NakamaFriendsManager Instance { get; private set; }
        public Dictionary<string, string> FriendStatus { get; private set; } = new Dictionary<string, string>();
        public List<IApiUser> Friends { get; private set; } = new List<IApiUser>();
        public List<IApiUser> FriendRequestsSent { get; private set; } = new List<IApiUser>();
        public List<IApiUser> FriendRequestReceived { get; private set; } = new List<IApiUser>();
        public bool LoadingFinished { get; private set; } = false;

        #endregion

        #region EVENTS

        public event Action onLoaded = null;
        public event Action<IApiUser> onFriendBecameOnline = null;
        public event Action<IApiUser> onFriendAdded = null;
        public event Action<IApiUser> onFriendRemoved = null;
        public event Action<IApiUser> onFriendRequestReceived = null;

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
            nakamaManager.onConnected += Connected;
            nakamaManager.onDisconnected += Disconnected;
            nakamaNotificationsManager.Subscribe(NakamaNotificationsManager.Code.FriendInviteWasAccepted, FriendInviteWasAccepted);
            nakamaNotificationsManager.Subscribe(NakamaNotificationsManager.Code.FriendInviteReceived, FriendInviteReceived);
            nakamaNotificationsManager.Subscribe(NakamaNotificationsManager.Code.FriendDeleted, FriendDeletedYou);
        }

        private void OnDestroy()
        {
            nakamaManager.onConnected -= Connected;
            nakamaManager.onDisconnected -= Disconnected;
            nakamaNotificationsManager.Unsubscribe(NakamaNotificationsManager.Code.FriendInviteWasAccepted, FriendInviteWasAccepted);
            nakamaNotificationsManager.Unsubscribe(NakamaNotificationsManager.Code.FriendInviteReceived, FriendInviteReceived);
            nakamaNotificationsManager.Unsubscribe(NakamaNotificationsManager.Code.FriendDeleted, FriendDeletedYou);
        }

        private void Connected()
        {
            nakamaManager.Socket.ReceivedStatusPresence += StatusChange;
            GetFriendList();
        }

        private void Disconnected()
        {
            nakamaManager.Socket.ReceivedStatusPresence -= StatusChange;
        }

        public void SubscribeToStatusChange(string userId, Action<string> action)
        {
            if (!onStatusChange.ContainsKey(userId))
                onStatusChange.Add(userId, null);

            onStatusChange[userId] += action;
        }

        public void UnsubscribeToStatusChange(string userId, Action<string> action)
        {
            if (onStatusChange.ContainsKey(userId))
                onStatusChange[userId] -= action;
        }

        private void StatusChange(IStatusPresenceEvent status)
        {
            foreach (var left in status.Leaves)
            {
                if (status.Joins.ToList().Exists(joined => joined.UserId == left.UserId))
                    continue;

                if (!FriendStatus.ContainsKey(left.UserId))
                    FriendStatus.Add(left.UserId, null);
                else
                    FriendStatus[left.UserId] = null;

                if (onStatusChange.ContainsKey(left.UserId))
                    onStatusChange[left.UserId]?.Invoke(null);
            }

            foreach (var joined in status.Joins)
            {
                if (!FriendStatus.ContainsKey(joined.UserId))
                    FriendStatus.Add(joined.UserId, null);

                FriendStatus[joined.UserId] = joined.Status;
                if (onStatusChange.ContainsKey(joined.UserId))
                    onStatusChange[joined.UserId]?.Invoke(joined.Status);

                if (!status.Leaves.ToList().Exists(leave => leave.UserId == joined.UserId))
                    if (Friends.Exists(friend => friend.Id == joined.UserId))
                        onFriendBecameOnline?.Invoke(Friends.First(friend => friend.Id == joined.UserId));
            }
        }

        private async void GetFriendList()
        {
            IApiFriendList result = await nakamaManager.Client.ListFriendsAsync(nakamaManager.Session, limit: FriendsLimit);
            foreach (IApiFriend friend in result.Friends)
            {
                switch ((NakamaFriendCode)friend.State)
                {
                    case NakamaFriendCode.Friends:
                        Friends.Add(friend.User);
                        break;
                    case NakamaFriendCode.InvitationSent:
                        FriendRequestsSent.Add(friend.User);
                        break;
                    case NakamaFriendCode.InvitationReceived:
                        FriendRequestReceived.Add(friend.User);
                        break;
                }
            }

            SubscribeToFriendStatus(Friends.Select(friend => friend.Id).ToList());
            LoadingFinished = true;
            onLoaded?.Invoke();
        }

        public void AcceptRequest(IApiUser user)
        {
            AddFriend(user);
            FriendRequestReceived.Remove(user);
            Friends.Add(user);
            onFriendAdded?.Invoke(user);
            SubscribeToFriendStatus(new List<string> { user.Id });
        }

        public CanAddFriendResult CanAddFriend(string username)
        {
            if (nakamaManager.Username == username)
                return CanAddFriendResult.CantAddYourself;
            else if (Friends.Exists(friend => friend.Username == username))
                return CanAddFriendResult.AlreadyYourFriend;
            else if (FriendRequestsSent.Exists(friendRequest => friendRequest.Username == username))
                return CanAddFriendResult.AlreadySentRequest;
            else
                return CanAddFriendResult.Success;
        }

        public async void AddFriend(string username)
        {
            IApiUser user = null;
            try
            {
                await nakamaManager.Client.AddFriendsAsync(nakamaManager.Session, new List<string> { }, new List<string> { username });
                user = FriendRequestReceived.FirstOrDefault(friendRequest => friendRequest.Username == username);
                if (user != null)
                {
                    Friends.Add(user);
                    FriendRequestReceived.Remove(user);
                    onFriendAdded?.Invoke(user);
                }
                else
                {
                    var result = await nakamaManager.Client.GetUsersAsync(nakamaManager.Session, new List<string>(), new List<string>() { username });
                    if (result.Users.Count() > 0)
                    {
                        user = result.Users.First();
                        FriendRequestsSent.Add(user);
                    }
                }
            }
            catch (ApiResponseException exception)
            {
                Debug.Log(exception);
            }

            SubscribeToFriendStatus(new List<string> { }, new List<string> { user.Username });
        }

        public async void AddFriend(IApiUser user)
        {
            try
            {
                await nakamaManager.Client.AddFriendsAsync(nakamaManager.Session, new List<string> { }, new List<string> { user.Username });
                if (FriendRequestReceived.Exists(friendRequest => friendRequest.Username == user.Username))
                {
                    Friends.Add(user);
                    FriendRequestReceived.Remove(user);
                    onFriendAdded?.Invoke(user);
                }
                else
                {
                    FriendRequestsSent.Add(user);
                }
            }
            catch (ApiResponseException exception)
            {
                Debug.Log(exception);
            }

            SubscribeToFriendStatus(new List<string> { }, new List<string> { user.Username });
        }

        public async void DeleteFriend(IApiUser user)
        {
            try
            {
                await nakamaManager.Client.DeleteFriendsAsync(nakamaManager.Session, new List<string> { user.Id });
                if (Friends.Exists(friend => friend.Username == user.Username))
                    Friends.Remove(user);
                else
                    FriendRequestReceived.Remove(user);

                nakamaNotificationsManager.SendNotification(NakamaNotificationsManager.Code.FriendDeleted, user.Id);
                onFriendRemoved?.Invoke(user);
            }
            catch (ApiResponseException exception)
            {
                Debug.Log(exception);
            }

            UnsubscribeToFriendStatus(new List<string> { user.Id });
        }

        private void FriendInviteWasAccepted(IApiNotification notification)
        {
            var user = FriendRequestsSent.Find(friendRequest => friendRequest.Id == notification.SenderId);
            if (user == null)
                return;

            FriendRequestsSent.Remove(user);
            Friends.Add(user);
            onFriendAdded?.Invoke(user);
            SubscribeToFriendStatus(new List<string> { user.Id });
        }

        private async void FriendInviteReceived(IApiNotification notification)
        {
            try
            {
                var result = await nakamaManager.Client.GetUsersAsync(nakamaManager.Session, new List<string> { notification.SenderId });
                if (result.Users.Count() <= 0)
                    return;

                var user = result.Users.First();
                FriendRequestReceived.Add(user);
                onFriendRequestReceived?.Invoke(user);

            }
            catch (ApiResponseException exception)
            {
                Debug.Log(exception);
            }
        }

        private void FriendDeletedYou(IApiNotification notification)
        {
            var user = Friends.Find(friend => friend.Id == notification.SenderId);
            if (user == null)
                user = FriendRequestsSent.Find(friendRequest => friendRequest.Id == notification.SenderId);

            if (user == null)
                return;

            Friends.Remove(user);
            FriendRequestsSent.Remove(user);
            onFriendRemoved?.Invoke(user);
            UnsubscribeToFriendStatus(new List<string> { user.Id });
        }

        private async void SubscribeToFriendStatus(IEnumerable<string> usersIds, IEnumerable<string> usernames = null)
        {
            var result = await nakamaManager.Socket.FollowUsersAsync(usersIds, usernames);
            foreach (IUserPresence presence in result.Presences)
            {
                if (!FriendStatus.ContainsKey(presence.UserId))
                    FriendStatus.Add(presence.UserId, null);

                FriendStatus[presence.UserId] = presence.Status;
                if (onStatusChange.ContainsKey(presence.UserId))
                    onStatusChange[presence.UserId]?.Invoke(presence.Status);
            }
        }

        private async void UnsubscribeToFriendStatus(IEnumerable<string> usersIds)
        {
            await nakamaManager.Socket.UnfollowUsersAsync(usersIds);
        }

        [ContextMenu(nameof(AddFriendDebug))]
        private void AddFriendDebug()
        {
            nakamaManager.Client.AddFriendsAsync(nakamaManager.Session, new List<string> { }, new List<string> { debugId });
        }

        [ContextMenu(nameof(DeleteFriendDebug))]
        private void DeleteFriendDebug()
        {
            nakamaManager.Client.DeleteFriendsAsync(nakamaManager.Session, new List<string> { }, new List<string> { debugId });
        }

        #endregion
    }
}
