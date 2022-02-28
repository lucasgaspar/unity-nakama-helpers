using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    public class NakamaNotificationUI : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private Text text = null;
        [SerializeField] private float showTime = 3f;

        #endregion

        #region BEHAVIORS

        public void Initialize(IApiNotification notification)
        {
            text.text = notification.Subject;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Invoke(nameof(Hide), showTime);
        }

        public void Hide()
        {
            Destroy(this.gameObject);
        }

        #endregion
    }
}
