using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaFriendsList : MonoBehaviour
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
            foreach (IApiUser friend in nakamaFriendsManager.Friends)
                CreateFriend(friend);

            nakamaFriendsManager.onFriendAdded += CreateFriend;
            nakamaFriendsManager.onFriendRemoved += FriendRemoved;
        }

        private void OnDestroy()
        {
            nakamaFriendsManager.onFriendAdded -= CreateFriend;
            nakamaFriendsManager.onFriendRemoved -= FriendRemoved;
        }

        private void CreateFriend(IApiUser user)
        {
            Instantiate(nakamaFriendPrefab, friendsParent).Initialize(user);
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

        #endregion
    }
}
