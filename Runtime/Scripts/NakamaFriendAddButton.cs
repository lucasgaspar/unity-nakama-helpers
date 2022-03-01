using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    [RequireComponent(typeof(Button))]
    public class NakamaFriendAddButton : MonoBehaviour
    {
        #region FIELDS

        private const string localizationSection = "friends/{0}";
        private const string ErrorCantAddYourself = "Cant add yourself";
        private const string ErrorAlreadyYourFriend = "Already your friend";
        private const string ErrorAlreadySentRequest = "Already sent request";

        [SerializeField] private NakamaUser nakamaUser = null;
        [SerializeField] private Text errorMessage = null;

        private NakamaFriendsManager nakamaFriendsManager = null;
        private Button button = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Add);
            nakamaUser.onChange += Changed;
        }

        private void Start()
        {
            nakamaFriendsManager = NakamaFriendsManager.Instance;
        }

        private void OnDestroy()
        {
            nakamaUser.onChange -= Changed;
        }

        private void Changed(IApiUser user)
        {
            button.interactable = user != null;
        }

        private void Add()
        {
            var result = nakamaFriendsManager.CanAddFriend(nakamaUser.User.Username);
            string message = string.Empty;
            switch (result)
            {
                case NakamaFriendsManager.CanAddFriendResult.Success:
                    nakamaFriendsManager.AddFriend(nakamaUser.User);
                    nakamaUser.Reset();
                    errorMessage.text = string.Empty;
                    break;
                case NakamaFriendsManager.CanAddFriendResult.CantAddYourself:
                    message = ErrorCantAddYourself;
                    break;
                case NakamaFriendsManager.CanAddFriendResult.AlreadyYourFriend:
                    message = ErrorAlreadyYourFriend;
                    break;
                case NakamaFriendsManager.CanAddFriendResult.AlreadySentRequest:
                    message = ErrorAlreadySentRequest;
                    break;
            }

            errorMessage.text = message != null ? message : string.Empty;
        }

        #endregion
    }
}
