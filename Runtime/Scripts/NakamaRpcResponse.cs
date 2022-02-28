using Newtonsoft.Json;

namespace Nakama.Helpers
{
    public class NakamaRpcResponse
    {
        #region FIELDS

        private const string DataKey = "data";
        private const string SuccessKey = "success";

        #endregion

        #region PROPERTIES

        [JsonProperty(DataKey)] public object Data { get; private set; }
        [JsonProperty(SuccessKey)] public bool Success { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public NakamaRpcResponse(object data, bool success)
        {
            Data = data;
            Success = success;
        }

        public T GetData<T>()
        {
            return Data.ToString().Deserialize<T>();
        }

        #endregion
    }
}
