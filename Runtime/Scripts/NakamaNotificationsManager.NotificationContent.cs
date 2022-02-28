using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nakama.Helpers
{
    public partial class NakamaNotificationsManager
    {
        public class NotificationContent
        {
            #region FIELDS

            private const string CodeKey = "code";
            private const string FromKey = "from";
            private const string ToKey = "to";
            private const string SubjectKey = "subject";
            private const string BodyKey = "body";
            private const string DataKey = "data";
            private const string PersistentKey = "persistent";
            private const string NakamaKey = "nakama";
            private const string FirebaseKey = "firebase";
            private const string DefaultSubject = "none";
            private const string DefaultBody = "none";

            #endregion

            #region PROPERTIES

            [JsonProperty(CodeKey)] public int Code { get; private set; }
            [JsonProperty(FromKey)] public string From { get; private set; }
            [JsonProperty(ToKey)] public string To { get; private set; }
            [JsonProperty(SubjectKey)] public string Subject { get; private set; }
            [JsonProperty(BodyKey)] public string Body { get; private set; }
            [JsonProperty(DataKey)] public IDictionary<string, string> Data { get; private set; }
            [JsonProperty(PersistentKey)] public bool Persistent { get; private set; }
            [JsonProperty(NakamaKey)] public bool Nakama { get; private set; }
            [JsonProperty(FirebaseKey)] public bool Firebase { get; private set; }

            #endregion

            #region CONSTRUCTORS

            [JsonConstructor]
            public NotificationContent(int code, string from, string to, string subject, string body, IDictionary<string, string> data, bool persistent, bool nakama, bool firebase)
            {
                Code = code;
                From = from;
                To = to;
                Subject = !string.IsNullOrEmpty(subject) ? subject : DefaultSubject;
                Body = !string.IsNullOrEmpty(body) ? body : DefaultBody;
                Data = data != null ? data : new Dictionary<string, string>();
                Persistent = persistent;
                Nakama = nakama;
                Firebase = firebase;
            }

            #endregion
        }
    }
}
