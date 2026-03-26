using UnityEngine;

namespace Game.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerView : MonoBehaviour
    {
        [SerializeField]
        private CharacterController characterController;

    }
}