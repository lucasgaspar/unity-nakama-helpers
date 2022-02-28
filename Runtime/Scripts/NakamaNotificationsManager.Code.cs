namespace Nakama.Helpers
{
    public partial class NakamaNotificationsManager
    {
        public enum Code
        {
            FriendInviteWasAccepted = -3,
            FriendInviteReceived = -2,
            FriendDeleted = 1,
            WalletUpdated = 2,
            MetadataUpdated = 3,
            ReceivedRaid = 4,
            ReceivedReward = 5,
            InvitedToHeist = 6
        }
    }
}
