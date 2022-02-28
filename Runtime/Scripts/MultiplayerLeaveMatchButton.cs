using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    public class MultiplayerLeaveMatchButton : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private Button button = null;
        [SerializeField] private string sceneName = "";

        private MultiplayerManager multiplayerManager = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            button.onClick.AddListener(LeaveMatch);
        }

        private void Start()
        {
            multiplayerManager = MultiplayerManager.Instance;
            multiplayerManager.onMatchLeave += ChangeScene;
        }

        private void OnDestroy()
        {
            multiplayerManager.onMatchLeave -= ChangeScene;
        }

        private void LeaveMatch()
        {
            button.interactable = false;
            multiplayerManager.LeaveMatchAsync();
        }

        private void ChangeScene()
        {
            SceneManager.LoadScene(sceneName);
        }

        #endregion
    }
}
