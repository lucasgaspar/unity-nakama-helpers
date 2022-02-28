using Newtonsoft.Json;

namespace Nakama.Helpers
{
    public static class JSONExtensions
    {
        #region BEHAVIORS

        public static string Serialize(this object obj, bool ignoreDefault = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DefaultValueHandling = ignoreDefault ? DefaultValueHandling.Ignore : DefaultValueHandling.Include;
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T Deserialize<T>(this string json)
        {
            return json == null ? default(T) : JsonConvert.DeserializeObject<T>(json);
        }

        #endregion
    }
}
