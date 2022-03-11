using UnityEngine;

namespace Nakama.Helpers
{
    [CreateAssetMenu(menuName = "Nakama/Connection")]
    public class NakamaConnectionData : ScriptableObject
    {
        #region FIELDS

        [SerializeField] private string scheme = "http";
        [SerializeField] private string host = "127.0.0.1";
        [SerializeField] private int port = 7350;
        [SerializeField] private string serverKey = "defaultkey";

        #endregion

        #region PROPERTIES

        public string Scheme { get => scheme; }
        public string Host { get => host; }
        public int Port { get => port; }
        public string ServerKey { get => serverKey; }

        #endregion
    }
}
