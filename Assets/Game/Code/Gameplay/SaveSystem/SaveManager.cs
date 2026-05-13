using System;
using System.Collections.Generic;
using Game.Core;
using Game.General;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class SaveManager : IInitializable
    {
        private readonly SaveSystemSettings _settings;
        private readonly CedarLogger _logger;

        private readonly Dictionary<Guid, SaveSlotData> _slots = new();

        public SaveManager(SaveSystemSettings settings, CedarLogger logger)
        {
            _settings = settings;
            _logger = logger;
        }
        
        public void Initialize()
        {
            LoadSlots();
        }

        public SaveSlotData CreateEmptySlot(GameDifficulty difficulty, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            var saveSlotID = Guid.NewGuid();
            var startLevelID = Const.Level.DefaultLevelID;
            
            var slotData = new SaveSlotData(saveSlotID, difficulty, startLevelID, spawnPosition, spawnRotation);
            _slots.Add(saveSlotID, slotData);
            
            _logger.Info(LogTag.Save, $"Created new empty save slot with difficulty {difficulty} (ID: {saveSlotID})");
            
            SaveSlots();
            
            return slotData;
        }

        public void DeleteSlot(Guid slotID)
        {
            _slots.Remove(slotID);
        }

        private void SaveSlots()
        {
            var slotsCount = _slots.Count;
            var slotsToSave = new SaveSlotData[slotsCount];
            
            _logger.Info(LogTag.Save, $"Saving {slotsCount} save slots.");

            try
            {
                var index = 0;
                foreach (var (saveSlotID, saveSlotData) in _slots)
                    slotsToSave[index++] = saveSlotData;
            
                var slotsJson = JsonConvert.SerializeObject(slotsToSave);
                _logger.Info(LogTag.Save, $"Save slots JSON: {slotsJson}");
            
                PlayerPrefs.SetString(Const.Save.SaveDataPlayerPrefsKey, slotsJson);
            }
            catch (Exception ex)
            {
                _logger.Fail(LogTag.Save, $"Failed to save slots. Exception: {ex}");
                throw;
            }
        }
        
        private void LoadSlots()
        {
            _logger.Info(LogTag.Save, "Loading save slots.");
            
            var slotsJson = PlayerPrefs.GetString(Const.Save.SaveDataPlayerPrefsKey, string.Empty);
            _logger.Info(LogTag.Save, $"Loaded save slots JSON: {slotsJson}");
            
            var slots = JsonConvert.DeserializeObject<SaveSlotData[]>(slotsJson);
            slots = slots ?? Array.Empty<SaveSlotData>();
            
            _slots.Clear();
            for (var i = 0; i < slots.Length; i++)
            {
                var slotData = slots[i];
                _slots[slotData.ID] = slotData;
            }
            
            _logger.Info(LogTag.Save, $"Deserialized {_slots.Count} save slots.");
        }
    }
}