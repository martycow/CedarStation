using Game.General;

namespace Game.Gameplay
{
    public class GameStartedEvent : IGameEvent
    {
        public GameContext GameContext;
        
        public GameStartedEvent(GameContext gameContext)
        {
            GameContext = gameContext;
        }
    }
}