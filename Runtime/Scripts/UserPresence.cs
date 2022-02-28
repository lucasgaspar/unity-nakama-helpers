namespace Nakama.Helpers
{
    public class UserPresence : IUserPresence
    {
        #region FIELDS

        private const string OfflineSessionId = "offlineSession";

        #endregion

        #region PROPERTIES

        public bool Persistence { get; private set; } = false;
        public string SessionId { get; private set; } = string.Empty;
        public string Status { get; private set; } = string.Empty;
        public string Username { get; private set; } = string.Empty;
        public string UserId { get; private set; } = string.Empty;

        #endregion

        #region CONSTRUCTORS

        public UserPresence(IUserPresence userPresence)
        {
            Persistence = userPresence.Persistence;
            SessionId = userPresence.SessionId;
            Status = userPresence.Status;
            Username = userPresence.Username;
            UserId = userPresence.UserId;
        }

        public UserPresence(string userId, string username)
        {
            UserId = userId;
            SessionId = OfflineSessionId;
            Username = username;
        }

        #endregion
    }
}
