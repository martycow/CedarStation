using System;

namespace Game.Gameplay
{
    [Serializable]
    public struct PlayerComponents
    {
        public bool IsSpawned;
        public PlayerSettings Settings;
        public CharacterVisual Visual;
        public CharacterMover Movement;
        public CharacterEmotions Emotion;
        
        public PlayerComponents(
            bool isSpawned,
            PlayerSettings settings, 
            CharacterVisual visual, 
            CharacterMover movement, 
            CharacterEmotions emotion)
        {
            IsSpawned = isSpawned;
            Settings = settings;
            Visual = visual;
            Movement = movement;
            Emotion = emotion;
        }

        public static PlayerComponents Empty => new(false, null, null, null, null);
    }
}