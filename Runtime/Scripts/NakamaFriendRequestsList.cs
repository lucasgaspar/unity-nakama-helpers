using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaFriendRequestsList : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private Transform friendsParent = null;
        [SerializeField] private NakamaUser nakamaFriendPrefab = null;

        private NakamaFriendsManager nakamaFriendsManager = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            nakamaFriendsManager = NakamaFriendsManager.Instance;
            foreach (IApiUser friend in nakamaFriendsManager.FriendRequestReceived)
                CreateFriend(friend);

            nakamaFriendsManager.onFriendAdded += FriendAdded;
            nakamaFriendsManager.onFriendRemoved += FriendRemoved;
            nakamaFriendsManager.onFriendRequestReceived += CreateFriend;
        }

        private void OnDestroy()
        {
            nakamaFriendsManager.onFriendAdded -= FriendAdded;
            nakamaFriendsManager.onFriendRemoved -= FriendRemoved;
            nakamaFriendsManager.onFriendRequestReceived -= CreateFriend;
        }

        private void FriendAdded(IApiUser user)
        {
            NakamaUser[] friendRequests = GetComponentsInChildren<NakamaUser>();
            foreach (NakamaUser nakamaUser in friendRequests)
            {
                if (nakamaUser.User.Username != user.Username)
                    continue;

                nakamaUser.gameObject.SetActive(false);
                return;
            }
        }

        private void FriendRemoved(IApiUser user)
        {
            NakamaUser[] friendRequests = GetComponentsInChildren<NakamaUser>();
            foreach (NakamaUser nakamaUser in friendRequests)
            {
                if (nakamaUser.User.Username != user.Username)
                    continue;

                nakamaUser.gameObject.SetActive(false);
                return;
            }
        }

        private void CreateFriend(IApiUser user)
        {
            Instantiate(nakamaFriendPrefab, friendsParent).Initialize(user);
        }

        #endregion
    }
}
