using System;
using System.Collections.Generic;
using Game.General;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class Level : ContextView<LevelData>
    {
        [SerializeField]
        private CinemachineCamera cinemachineCamera;

        [SerializeField]
        private List<VolumeBox> spawnedVolumeBoxes;
        
        private GameObject _collidersParent;
        private GameObject _cinemachineParent;

        protected override void Init()
        {
            gameObject.name = $"Level_{Context.TechName}";
            
            InitRoots();
            SpawnVolumeBoxes();
        }

        protected override void UpdateView()
        {
            
        }

        private void InitRoots()
        {
            _collidersParent = new GameObject("Colliders");
            _collidersParent.transform.SetParent(transform);

            _cinemachineParent = new GameObject("Cinemachine");
            _cinemachineParent.transform.SetParent(transform);
        }
        
        private void SpawnVolumeBoxes()
        {
            spawnedVolumeBoxes.Clear();
            
            // Player Spawn Zones
            foreach (var volumeData in Context.PlayerSpawnZones)
            {
                var go = new GameObject();
                go.transform.SetParent(_collidersParent.transform);
                
                var volumeBox = go.AddComponent<VolumeBox>();
                volumeBox.Setup(volumeData, ContextViewUpdateType.OnSetup | ContextViewUpdateType.EveryFrame);

                spawnedVolumeBoxes.Add(volumeBox);
            }
            
            // Teleports
            foreach (var teleportData in Context.Teleports)
            {
                var volumeData = teleportData.TeleportZone;
                
                var go = new GameObject();
                go.transform.SetParent(_collidersParent.transform);
                var volumeBox = go.AddComponent<VolumeBox>();
                volumeBox.Setup(volumeData, ContextViewUpdateType.OnSetup | ContextViewUpdateType.EveryFrame);
                
                spawnedVolumeBoxes.Add(volumeBox);
            }
            
            // Other Spawn Zones
            foreach (var volumeData in Context.OtherSpawnZones)
            {
                var go = new GameObject();
                go.transform.SetParent(_collidersParent.transform);
                var volumeBox = go.AddComponent<VolumeBox>();
                volumeBox.Setup(volumeData, ContextViewUpdateType.OnSetup | ContextViewUpdateType.EveryFrame);
                
                spawnedVolumeBoxes.Add(volumeBox);
            }
        }
    }
}