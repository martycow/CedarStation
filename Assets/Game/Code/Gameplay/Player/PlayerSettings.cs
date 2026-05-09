using UnityEngine;

namespace Game.Gameplay
{
    [CreateAssetMenu(menuName = "Cedar Station/System/Create Player Settings", fileName = "Settings_Player")]
    public sealed class PlayerSettings : ScriptableObject
    {
        [SerializeField] private CharacterMover characterMoverPrefab;
        [SerializeField] private float moveSpeed = 0.15f;
        [SerializeField] private float jumpCooldown = 0.3f;
        [SerializeField] private float jumpForce = 5f;

        public CharacterMover CharacterMoverPrefab => characterMoverPrefab;
        public float MoveSpeed => moveSpeed;
        public float JumpCooldown => jumpCooldown;
        public float JumpForce => jumpForce;
    }
}