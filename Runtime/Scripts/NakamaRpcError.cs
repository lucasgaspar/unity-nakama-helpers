using Newtonsoft.Json;

namespace Nakama.Helpers
{
    public class NakamaRpcError
    {
        #region FIELDS

        private const string ErrorKey = "error";

        #endregion

        #region PROPERTIES

        [JsonProperty(ErrorKey)] public string Error { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public NakamaRpcError(string error)
        {
            Error = error;
        }

        #endregion
    }
}
