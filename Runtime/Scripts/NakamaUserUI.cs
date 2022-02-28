using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    public class NakamaUserUI : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private NakamaUser nakamaUser = null;
        [SerializeField] private Image avatar = null;
        [SerializeField] private Sprite defaultImage = null;
        [SerializeField] private Text displayName = null;
        [SerializeField] private Text username = null;
        [SerializeField] private Text level = null;

        private NakamaUserManager nakamaUserManager = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            nakamaUserManager = NakamaUserManager.Instance;
            nakamaUser.onChange += Changed;
            if (nakamaUser == null)
                Clear();
            else
                Changed(nakamaUser.User);
        }

        private void OnDestroy()
        {
            nakamaUser.onChange -= Changed;
        }

        private void Changed(IApiUser user)
        {
            if (user == null)
            {
                Clear();
                return;
            }

            if (avatar != null)
            {
                int index = 0;
                int.TryParse(user.AvatarUrl, out index);
                avatar.sprite = profilePictures.Pictures[index];
            }

            if (displayName != null)
                displayName.text = user.DisplayName == null ? DefaultName : user.DisplayName;

            if (username != null)
                username.text = user.Username;

            if (level != null)
            {
                MetadataData metadata = nakamaUserManager.Metadata.Deserialize<MetadataData>();
                level.text = string.Format(LevelFormat, metadata == null || metadata.Level == 0 ? DefaultLevel : metadata.Level);
            }
        }

        private void Clear()
        {
            if (avatar != null)
                avatar.sprite = defaultImage;

            if (displayName != null)
                displayName.text = string.Empty;

            if (username != null)
                username.text = string.Empty;

            if (level != null)
                level.text = string.Empty;
        }

        #endregion
    }
}
