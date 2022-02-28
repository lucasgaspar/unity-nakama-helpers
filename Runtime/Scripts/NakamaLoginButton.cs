using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    public class NakamaLoginButton : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private Button button = null;

        private NakamaManager nakamaManager = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            nakamaManager = NakamaManager.Instance;
            nakamaManager.onConnecting += Connecting;
            nakamaManager.onLoginFail += LoginFailed;
            button.onClick.AddListener(Login);
        }

        private void OnDestroy()
        {
            nakamaManager.onConnecting -= Connecting;
            nakamaManager.onLoginFail -= LoginFailed;
            button.onClick.RemoveListener(Login);

        }

        private void Login()
        {
            nakamaManager.LoginWithDevice();
        }

        private void Connecting()
        {
            gameObject.SetActive(false);
        }

        private void LoginFailed()
        {
            gameObject.SetActive(true);
        }

        #endregion
    }
}
