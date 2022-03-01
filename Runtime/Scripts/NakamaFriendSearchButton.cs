using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    [RequireComponent(typeof(Button))]
    public class NakamaFriendSearchButton : MonoBehaviour
    {
        #region FIELDS

        private const string ErrorUserNotFound = "User not found";
        private const int RequiredCharacters = 10;

        [SerializeField] private InputField inputField = null;
        [SerializeField] private Text errorMessage = null;
        [SerializeField] private NakamaUser nakamaUser = null;

        private NakamaManager nakamaManager = null;
        private Button button = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SearchUser);
            inputField.onValueChanged.AddListener(UpdateButtonStatus);
        }

        private void Start()
        {
            nakamaManager = NakamaManager.Instance;
        }

        private void OnDestroy()
        {
            inputField.onValueChanged.RemoveListener(UpdateButtonStatus);
        }

        private void UpdateButtonStatus(string newText)
        {
            button.interactable = newText.Length == RequiredCharacters;
            errorMessage.text = string.Empty;
            nakamaUser.Reset();
        }

        private async void SearchUser()
        {
            try
            {
                var result = await nakamaManager.Client.GetUsersAsync(nakamaManager.Session, new List<string> { }, new List<string> { inputField.text });
                if (result.Users.Count() > 0)
                {
                    nakamaUser.Initialize(result.Users.First());
                }
                else
                {
                    errorMessage.text = ErrorUserNotFound;
                }
            }
            catch (ApiResponseException exception)
            {
                Debug.Log(exception);
            }
        }

        #endregion
    }
}
