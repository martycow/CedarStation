using Game.General;

namespace Game.Gameplay
{
    public class PlayerCreatedEvent : IGameEvent
    {
        public readonly PlayerView Player;

        public PlayerCreatedEvent(PlayerView player)
        {
            Player = player;
        }
    }
}