using UnityEngine;

namespace Nakama.Helpers
{
    public class MultiplayerIdentity : MonoBehaviour
    {
        #region FIELDS

        private static int currentId = 0;

        [SerializeField] private bool autoAssignId = true;

        #endregion

        #region PROPERTIES

        public string Id { get; private set; }

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            if (autoAssignId)
                AssignIdentity();
        }

        private void AssignIdentity()
        {
            Id = currentId++.ToString();
        }

        public void SetId(string id)
        {
            Id = id;
        }

        public static void ResetIds()
        {
            currentId = default(int);
        }

        #endregion
    }
}
