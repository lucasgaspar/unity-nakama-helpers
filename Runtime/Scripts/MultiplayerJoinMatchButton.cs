using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    public class MultiplayerJoinMatchButton : MonoBehaviour
    {
        #region FIELDS


        [SerializeField] private Button button = null;
        [SerializeField] private string sceneName = "";

        private MultiplayerManager multiplayerManager = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            button.onClick.AddListener(FindMatch);
        }

        private void Start()
        {
            multiplayerManager = MultiplayerManager.Instance;
            multiplayerManager.onMatchJoin += ChangeScene;
        }

        private void OnDestroy()
        {
            multiplayerManager.onMatchJoin -= ChangeScene;
        }

        private void FindMatch()
        {
            button.interactable = false;
            multiplayerManager.JoinPublicMatch();
        }

        private void ChangeScene()
        {
            SceneManager.LoadScene(sceneName);
            MultiplayerIdentity.ResetIds();
        }

        #endregion
    }
}
