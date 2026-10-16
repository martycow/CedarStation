using Unity.Cinemachine;
using UnityEngine;

namespace Game.Gameplay
{
    [AddComponentMenu("Cedar Station/Level/Level Scene Root")]
    public sealed class LevelSceneRoot : MonoBehaviour
    {
        [SerializeField]
        private CinemachineCamera cinemachineCamera;

        public CinemachineCamera CinemachineCamera => cinemachineCamera;
    }
}
