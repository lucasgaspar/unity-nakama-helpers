using Newtonsoft.Json;

namespace Nakama.Helpers
{
    public class ApiUser : IApiUser
    {
        #region FIELDS

        private const string AppleIdKey = "appleId";
        private const string AvatarUrlKey = "avatarUrl";
        private const string CreateTimeKey = "createTime";
        private const string DisplayNameKey = "displayName";
        private const string EdgeCountKey = "edgeCount";
        private const string FacebookIdKey = "facebookId";
        private const string FacebookInstantGameIdKey = "facebookInstantGameId";
        private const string GameCenterId = "gamecenterId";
        private const string GoogleIdKey = "googleId";
        private const string IdKey = "userId";
        private const string LangTagKey = "langTag";
        private const string LocationKey = "location";
        private const string MetadataKey = "metadata";
        private const string OnlineKey = "online";
        private const string SteamIdKey = "steamId";
        private const string TimezoneKey = "timezone";
        private const string UpdateTimeKey = "updateTime";
        private const string UsernameKey = "username";

        #endregion

        #region PROPERTIES

        [JsonProperty(AppleIdKey)] public string AppleId { get; private set; }
        [JsonProperty(AvatarUrlKey)] public string AvatarUrl { get; private set; }
        [JsonProperty(CreateTimeKey)] public string CreateTime { get; private set; }
        [JsonProperty(DisplayNameKey)] public string DisplayName { get; private set; }
        [JsonProperty(EdgeCountKey)] public int EdgeCount { get; private set; }
        [JsonProperty(FacebookIdKey)] public string FacebookId { get; private set; }
        [JsonProperty(FacebookInstantGameIdKey)] public string FacebookInstantGameId { get; private set; }
        [JsonProperty(GameCenterId)] public string GamecenterId { get; private set; }
        [JsonProperty(GoogleIdKey)] public string GoogleId { get; private set; }
        [JsonProperty(IdKey)] public string Id { get; private set; }
        [JsonProperty(LangTagKey)] public string LangTag { get; private set; }
        [JsonProperty(LocationKey)] public string Location { get; private set; }
        [JsonProperty(MetadataKey)] public string Metadata { get; private set; }
        [JsonProperty(OnlineKey)] public bool Online { get; private set; }
        [JsonProperty(SteamIdKey)] public string SteamId { get; private set; }
        [JsonProperty(TimezoneKey)] public string Timezone { get; private set; }
        [JsonProperty(UpdateTimeKey)] public string UpdateTime { get; private set; }
        [JsonProperty(UsernameKey)] public string Username { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public ApiUser(string appleId, string avatarUrl, string createTime, string displayName, int edgeCount, string facebookId, string facebookInstantGameId, string gamecenterId, string googleId, string id, string langTag, string location, object metadata, bool online, string steamId, string timezone, string updateTime, string username)
        {
            AppleId = appleId;
            AvatarUrl = avatarUrl;
            CreateTime = createTime;
            DisplayName = displayName;
            EdgeCount = edgeCount;
            FacebookId = facebookId;
            FacebookInstantGameId = facebookInstantGameId;
            GamecenterId = gamecenterId;
            GoogleId = googleId;
            Id = id;
            LangTag = langTag;
            Location = location;
            Metadata = metadata.ToString();
            Online = online;
            SteamId = steamId;
            Timezone = timezone;
            UpdateTime = updateTime;
            Username = username;
        }

        #endregion
    }
}
