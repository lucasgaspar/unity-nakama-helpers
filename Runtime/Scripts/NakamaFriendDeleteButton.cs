using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    [RequireComponent(typeof(Button))]
    public class NakamaFriendDeleteButton : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private NakamaUser nakamaUser = null;

        private NakamaFriendsManager nakamaFriendsManager = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Delete);
        }

        private void Start()
        {
            nakamaFriendsManager = NakamaFriendsManager.Instance;
        }

        private void Delete()
        {
            nakamaFriendsManager.DeleteFriend(nakamaUser.User);
            nakamaUser.gameObject.SetActive(false);
        }

        #endregion
    }
}
