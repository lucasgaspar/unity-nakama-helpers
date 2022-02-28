namespace Nakama.Helpers
{
    public partial class MultiplayerManager
    {
        public enum Code
        {
            PlayersStatus = 0,
            PlayerJoined = 1,
            PlayerLeft = 2,
            PlayerReady = 3,
            PlayerEscaping = 4,
            PlayerMovement = 5,
            PlayerMessage = 6,
            PlayerPowerUp = 7,
            PlayerSeen = 8,
            PlayerStatusEffect = 9,
            MissionInitialize = 10,
            MissionStart = 11,
            MissionSucceeded = 12,
            MissionFailed = 13,
            TimerLobby = 20,
            TimerGame = 21,
            ObjectPickUp = 30,
            ObjectStolen = 31,
            ObjectInteract = 32,
            ObjectCamera = 33,
            ObjectsLobby = 34,
            AlarmActivated = 40,
            MatchMakePublic = 50
        }
    }
}
