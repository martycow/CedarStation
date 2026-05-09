using System;

namespace Game.Gameplay
{
    [Serializable]
    public struct PlayerComponents
    {
        public PlayerSettings Settings;
        public CharacterVisual Visual;
        public CharacterMover Movement;
        public CharacterEmotions Emotion;
        
        public PlayerComponents(PlayerSettings settings, CharacterVisual visual, CharacterMover movement, CharacterEmotions emotion)
        {
            Settings = settings;
            Visual = visual;
            Movement = movement;
            Emotion = emotion;
        }
        
        public static PlayerComponents Empty => new(null, null, null, null);
    }
}