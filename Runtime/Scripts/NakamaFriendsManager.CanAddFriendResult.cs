namespace Nakama.Helpers
{
    public partial class NakamaFriendsManager
    {
        public enum CanAddFriendResult
        {
            Success = 0,
            CantAddYourself = 1,
            AlreadyYourFriend = 2,
            AlreadySentRequest = 3
        }
    }
}
