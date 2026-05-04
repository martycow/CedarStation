using System;
using System.Collections.Generic;
using Cedar.Core;
using Game.General;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class Level : BaseView<LevelData>
    {
        private readonly Dictionary<Guid, VolumeBox> _spawnedVolumeBoxes = new();
        
        protected override void Init()
        {
            gameObject.name = Context.TechName;
            
            SpawnVolumeBoxes();
        }

        public override void UpdateView()
        {
            
        }
        
        private void SpawnVolumeBoxes()
        {
            _spawnedVolumeBoxes.Clear();
            
            // Player Spawn Zones
            for (var i = 0; i < Context.PlayerSpawnZones.Length; i++)
            {
                var zone = Context.PlayerSpawnZones[i];

                var go = new GameObject();
                go.transform.SetParent(transform);
                var volumeBox = go.AddComponent<VolumeBox>();
                volumeBox.Setup(zone, ViewUpdateType.OnSetup | ViewUpdateType.EveryFrame);
                
                _spawnedVolumeBoxes[zone.ID] = volumeBox;
            }
            
            // Teleports
            for (var i = 0; i < Context.Teleports.Length; i++)
            {
                var zone = Context.Teleports[i].TeleportZone;
                
                var go = new GameObject();
                go.transform.SetParent(transform);
                var volumeBox = go.AddComponent<VolumeBox>();
                volumeBox.Setup(zone, ViewUpdateType.OnSetup | ViewUpdateType.EveryFrame);
                
                _spawnedVolumeBoxes[zone.ID] = volumeBox;
            }
            
            // Other Spawn Zones
            for (var i = 0; i < Context.OtherSpawnZones.Length; i++)
            {
                var zone = Context.OtherSpawnZones[i];
                
                var go = new GameObject();
                go.transform.SetParent(transform);
                var volumeBox = go.AddComponent<VolumeBox>();
                volumeBox.Setup(zone, ViewUpdateType.OnSetup | ViewUpdateType.EveryFrame);
                
                _spawnedVolumeBoxes[zone.ID] = volumeBox;
            }
            
            Context.Logger.Info(SystemTag.Level, $"Spawned {_spawnedVolumeBoxes.Count} volume boxes for level {Context.TechName}");
        }
    }
}