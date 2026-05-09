using Game.General;

namespace Game.Gameplay
{
    public class PlayerCreatedEvent : IGameEvent
    {
        public readonly CharacterMover CharacterMover;

        public PlayerCreatedEvent(CharacterMover characterMover)
        {
            CharacterMover = characterMover;
        }
    }
}