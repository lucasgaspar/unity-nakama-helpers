using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaAutoLogin : MonoBehaviour
    {
        #region FIELDS

        private const string TestAccountName = "TestAccount";

        [SerializeField] private float retryTime = 5f;

        private NakamaManager nakamaManager = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            nakamaManager = NakamaManager.Instance;
            nakamaManager.onLoginFail += LoginFailed;
            TryLogin();
        }

        private void OnDestroy()
        {
            nakamaManager.onLoginFail -= LoginFailed;
        }

        private void TryLogin()
        {
            nakamaManager.LoginWithCustomId(TestAccountName);
        }

        private void LoginFailed()
        {

        }

        #endregion
    }
}
