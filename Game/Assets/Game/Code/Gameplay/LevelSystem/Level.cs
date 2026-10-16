using System.Collections.Generic;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class Level : ContextView<LevelData>
    {
        private readonly List<VolumeBox> _spawnedVolumeBoxes = new();
        private GameObject _collidersParent;

        protected override void Init()
        {
            gameObject.name = $"Level_{Context.TechName}";
            InitRoots();
            SpawnVolumeBoxes();
        }

        protected override void UpdateView() { }

        private void InitRoots()
        {
            _collidersParent = new GameObject("Colliders");
            _collidersParent.transform.SetParent(transform);
        }

        private void SpawnVolumeBoxes()
        {
            _spawnedVolumeBoxes.Clear();

            foreach (var volumeData in Context.PlayerSpawnZones)
            {
                var go = new GameObject();
                go.transform.SetParent(_collidersParent.transform);
                var volumeBox = go.AddComponent<VolumeBox>();
                volumeBox.Setup(volumeData, ContextViewUpdateType.OnSetup | ContextViewUpdateType.EveryFrame);
                _spawnedVolumeBoxes.Add(volumeBox);
            }

            foreach (var teleportData in Context.Teleports)
            {
                var go = new GameObject();
                go.transform.SetParent(_collidersParent.transform);
                var volumeBox = go.AddComponent<VolumeBox>();
                volumeBox.Setup(teleportData.TeleportZone, ContextViewUpdateType.OnSetup | ContextViewUpdateType.EveryFrame);
                _spawnedVolumeBoxes.Add(volumeBox);
            }

            foreach (var volumeData in Context.OtherSpawnZones)
            {
                var go = new GameObject();
                go.transform.SetParent(_collidersParent.transform);
                var volumeBox = go.AddComponent<VolumeBox>();
                volumeBox.Setup(volumeData, ContextViewUpdateType.OnSetup | ContextViewUpdateType.EveryFrame);
                _spawnedVolumeBoxes.Add(volumeBox);
            }
        }
    }
}
