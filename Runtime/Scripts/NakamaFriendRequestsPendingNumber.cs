using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    public class NakamaFriendRequestsPendingNumber : MonoBehaviour
    {
        #region FIELDS

        private const int MaximumNumber = 99;

        [SerializeField] private GameObject container = null;
        [SerializeField] private Text numberText = null;

        private NakamaFriendsManager nakamaFriendsManager = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            nakamaFriendsManager = NakamaFriendsManager.Instance;
            nakamaFriendsManager.onFriendAdded += FriendsChanged;
            nakamaFriendsManager.onFriendRemoved += FriendsChanged;
            nakamaFriendsManager.onFriendRequestReceived += FriendsChanged;
            nakamaFriendsManager.onLoaded += SetNumber;
        }

        private void Start()
        {
            SetNumber();
        }

        private void OnDestroy()
        {
            nakamaFriendsManager.onFriendAdded -= FriendsChanged;
            nakamaFriendsManager.onFriendRemoved -= FriendsChanged;
            nakamaFriendsManager.onFriendRequestReceived -= FriendsChanged;
            nakamaFriendsManager.onLoaded -= SetNumber;
        }

        private void FriendsChanged(IApiUser user)
        {
            SetNumber();
        }

        private void SetNumber()
        {
            int friendRequestsCount = Mathf.Min(nakamaFriendsManager.FriendRequestReceived.Count, MaximumNumber);
            container.gameObject.SetActive(friendRequestsCount > 0);
            numberText.text = friendRequestsCount.ToString();
        }

        #endregion
    }
}
