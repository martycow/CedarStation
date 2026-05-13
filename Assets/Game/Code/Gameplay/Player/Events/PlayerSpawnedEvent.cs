using Game.General;

namespace Game.Gameplay
{
    public class PlayerSpawnedEvent : IGameEvent
    {
        public readonly CharacterMover CharacterMover;

        public PlayerSpawnedEvent(CharacterMover characterMover)
        {
            CharacterMover = characterMover;
        }
    }
}