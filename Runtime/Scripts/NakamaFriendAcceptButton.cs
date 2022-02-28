using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    [RequireComponent(typeof(Button))]
    public class NakamaFriendAcceptButton : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private NakamaUser nakamaUser = null;

        private NakamaFriendsManager nakamaFriendsManager = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Accept);
        }

        private void Start()
        {
            nakamaFriendsManager = NakamaFriendsManager.Instance;
        }

        private void Accept()
        {
            nakamaFriendsManager.AcceptRequest(nakamaUser.User);
            nakamaUser.gameObject.SetActive(false);
        }

        #endregion
    }
}
