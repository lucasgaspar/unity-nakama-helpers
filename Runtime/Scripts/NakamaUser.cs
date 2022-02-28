using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaUser : MonoBehaviour
    {
        #region FIELDS

        private NakamaManager nakamaManager = null;

        #endregion

        #region EVENTS

        public event Action<IApiUser> onChange = null;

        #endregion

        #region PROPERTIES

        public IApiUser User { get; private set; } = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            nakamaManager = NakamaManager.Instance;
        }

        public void Initialize(IApiUser user)
        {
            onChange?.Invoke(user);
            User = user;
        }

        public void InitializeByUserId(string userId)
        {
            GetUserFromDatabase(userId: userId);
        }

        public void InitializeByUsername(string username)
        {
            GetUserFromDatabase(username: username);
        }

        private async void GetUserFromDatabase(string userId = null, string username = null)
        {
            List<string> userIds = userId == null ? new List<string>() : new List<string>() { userId };
            List<string> usernames = username == null ? new List<string>() : new List<string>() { username };
            var result = await nakamaManager.Client.GetUsersAsync(nakamaManager.Session, userIds, usernames);
            if (result.Users.Count() > 0)
                Initialize(result.Users.First());
        }

        public void Reset()
        {
            onChange?.Invoke(null);
            User = null;
        }

        #endregion
    }
}
