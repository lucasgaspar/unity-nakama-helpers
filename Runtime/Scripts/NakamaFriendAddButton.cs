using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    [RequireComponent(typeof(Button))]
    public class NakamaFriendAddButton : MonoBehaviour
    {
        #region FIELDS

        private const string localizationSection = "friends/{0}";
        private const string ErrorCantAddYourself = "own_id";
        private const string ErrorAlreadyYourFriend = "friend_already";
        private const string ErrorAlreadySentRequest = "invite_sent";

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
                    message = LocalizationManager.GetTranslation(string.Format(localizationSection, ErrorCantAddYourself));
                    break;
                case NakamaFriendsManager.CanAddFriendResult.AlreadyYourFriend:
                    message = LocalizationManager.GetTranslation(string.Format(localizationSection, ErrorAlreadyYourFriend));
                    break;
                case NakamaFriendsManager.CanAddFriendResult.AlreadySentRequest:
                    message = LocalizationManager.GetTranslation(string.Format(localizationSection, ErrorAlreadySentRequest));
                    break;
            }

            errorMessage.text = message != null ? message : string.Empty;
        }

        #endregion
    }
}
