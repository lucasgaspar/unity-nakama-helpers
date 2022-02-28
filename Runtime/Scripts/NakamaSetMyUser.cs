using UnityEngine;

namespace Nakama.Helpers
{
    [RequireComponent(typeof(NakamaUser))]
    public class NakamaSetMyUser : MonoBehaviour
    {
        #region FIELDS

        private NakamaUserManager nakamaUserManager = null;
        private NakamaUser nakamaUser = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            nakamaUser = GetComponent<NakamaUser>();
        }

        public void Start()
        {
            nakamaUserManager = NakamaUserManager.Instance;
            nakamaUserManager.onLoaded += SetInformation;
            if (nakamaUserManager.LoadingFinished)
                SetInformation();
        }

        private void OnDestroy()
        {
            nakamaUserManager.onLoaded -= SetInformation;
        }

        private void SetInformation()
        {
            nakamaUser.Initialize(nakamaUserManager.User);
        }

        #endregion
    }
}
