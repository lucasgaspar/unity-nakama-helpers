using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    [RequireComponent(typeof(Image))]
    public class NakamaFriendStatus : MonoBehaviour
    {
        #region FIELDS

        private const string Online = "Online";
        private const string Busy = "Busy";

        [SerializeField] private NakamaUser nakamaUser = null;
        [SerializeField] private Image icon = null;
        [SerializeField] private Sprite onlineIcon = null;
        [SerializeField] private Sprite busyIcon = null;
        [SerializeField] private Sprite offlineIcon = null;

        private NakamaFriendsManager nakamaFriendsManager = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            nakamaFriendsManager = NakamaFriendsManager.Instance;
            if (nakamaUser.User == null)
                return;

            nakamaFriendsManager.SubscribeToStatusChange(nakamaUser.User.Id, SetStatus);
            SetStatus(FindStatus(nakamaUser.User));
        }

        private void OnDestroy()
        {
            nakamaFriendsManager.UnsubscribeToStatusChange(nakamaUser.User.Id, SetStatus);
        }

        private string FindStatus(IApiUser user)
        {
            if (nakamaUser.User == null)
                return null;

            if (!nakamaFriendsManager.FriendStatus.ContainsKey(nakamaUser.User.Id))
                return null;

            return nakamaFriendsManager.FriendStatus[nakamaUser.User.Id];
        }

        private void SetStatus(string status)
        {
            if (status == null)
            {
                icon.sprite = offlineIcon;
                return;
            }

            switch (status)
            {
                case Busy:
                    icon.sprite = busyIcon;
                    break;
                case Online:
                    icon.sprite = onlineIcon;
                    break;
                default:
                    icon.sprite = offlineIcon;
                    break;
            }
        }

        #endregion
    }
}
