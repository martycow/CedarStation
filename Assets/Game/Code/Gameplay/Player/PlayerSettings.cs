using UnityEngine;

namespace Game.Gameplay
{
    [CreateAssetMenu(menuName = "Cedar Station/System/Create Player Settings", fileName = "Settings_Player")]
    public sealed class PlayerSettings : ScriptableObject
    {
        [SerializeField] private Player playerPrefab;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpCooldown = 0.3f;
        [SerializeField] private float jumpForce = 5f;

        public Player PlayerPrefab => playerPrefab;
        public float MoveSpeed => moveSpeed;
        public float JumpCooldown => jumpCooldown;
        public float JumpForce => jumpForce;
    }
}