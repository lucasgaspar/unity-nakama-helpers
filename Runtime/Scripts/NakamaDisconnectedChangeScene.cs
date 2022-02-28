using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nakama.Helpers
{
    public class NakamaDisconnectedChangeScene : MonoBehaviour
    {
        #region FIELDS

        private NakamaManager nakamaManager = null;

        [SerializeField] private string sceneName = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            nakamaManager.onDisconnected += Disconnected;
        }

        private void OnDestroy()
        {
            nakamaManager.onDisconnected -= Disconnected;
        }

        private void Disconnected()
        {
            SceneManager.LoadScene(sceneName);
        }

        #endregion
    }
}
