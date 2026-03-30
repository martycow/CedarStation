using UnityEngine;

namespace Game.Gameplay
{
    [CreateAssetMenu(menuName = "Settings/Create PlayerSettings", fileName = "PlayerSettings", order = 0)]
    public sealed class PlayerSettings : ScriptableObject
    {
        [SerializeField] private PlayerView playerPrefab;
        [SerializeField] private float moveSpeed = 5f;
        
        public PlayerView PlayerPrefab => playerPrefab;
        public float MoveSpeed => moveSpeed;
    }
}