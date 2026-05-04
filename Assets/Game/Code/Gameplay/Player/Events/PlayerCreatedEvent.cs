using Game.General;

namespace Game.Gameplay
{
    public class PlayerCreatedEvent : IGameEvent
    {
        public readonly Player Player;

        public PlayerCreatedEvent(Player player)
        {
            Player = player;
        }
    }
}