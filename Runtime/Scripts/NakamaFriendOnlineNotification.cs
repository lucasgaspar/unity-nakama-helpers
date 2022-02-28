using System.Collections.Generic;
using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaFriendOnlineNotification : MonoBehaviour
    {
        #region FIELDS

        private const float AppearDuration = 0.25f;
        private const float ShowDuration = 2f;
        private const float HideDuration = 0.25f;

        [SerializeField] private NakamaUser nakamaUser = null;
        [SerializeField] private RectTransform container = null;
        [SerializeField] private RectTransform showPosition = null;
        [SerializeField] private RectTransform hidePosition = null;

        private NakamaFriendsManager nakamaFriendsManager = null;
        private Queue<IApiUser> pendingNotifications = new Queue<IApiUser>();
        private bool isShowing = false;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            nakamaFriendsManager = NakamaFriendsManager.Instance;
            nakamaFriendsManager.onFriendBecameOnline += FriendBecameOnline;
        }

        private void OnDestroy()
        {
            nakamaFriendsManager.onFriendBecameOnline -= FriendBecameOnline;
        }

        private void FriendBecameOnline(IApiUser user)
        {
            pendingNotifications.Enqueue(user);
            if (!isShowing)
                Show();
        }

        private void Show()
        {
            isShowing = true;
            nakamaUser.Initialize(pendingNotifications.Dequeue());
            //container.DOAnchorPos(showPosition.anchoredPosition, AppearDuration).OnComplete(ShowComplete);
        }

        private void ShowComplete()
        {
            Invoke(nameof(Hide), ShowDuration);
        }

        private void Hide()
        {
            //container.DOAnchorPos(hidePosition.anchoredPosition, HideDuration).OnComplete(HideComplete);
        }

        private void HideComplete()
        {
            if (pendingNotifications.Count > 0)
                Show();
            else
                isShowing = false;
        }

        #endregion
    }
}
