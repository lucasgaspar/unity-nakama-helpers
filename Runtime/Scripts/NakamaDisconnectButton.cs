using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    public class NakamaDisconnectButton : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private Button button = null;

        private NakamaManager nakamaManager = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            nakamaManager = NakamaManager.Instance;
            button.onClick.AddListener(Disconect);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Disconect);
        }

        private void Disconect()
        {
            nakamaManager.LogOut();
        }

        #endregion
    }
}
