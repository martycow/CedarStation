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

        public SaveSlotData CreateEmptySlot(GameDifficulty difficulty, Guid levelID, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            var saveSlotID = Guid.NewGuid();
            
            var slotData = new SaveSlotData(saveSlotID, difficulty, levelID, spawnPosition, spawnRotation);
            _slots.Add(saveSlotID, slotData);
            
            _logger.Info(LogTag.Save, $"Created new save slot in memory, not written to disk yet (ID: {saveSlotID})");
            
            return slotData;
        }

        public void Save(SaveSlotData slotData)
        {
            slotData.IsFresh = false;
            _slots[slotData.ID] = slotData;
            SaveSlots();
        }
        
        public SaveSlotData GetSlot(Guid slotID)
        {
            if (_slots.TryGetValue(slotID, out var slotData))
                return slotData;
            
            _logger.Warn(LogTag.Save, $"Save slot with ID {slotID} not found.");
            return null;
        }

        public void DeleteSlot(Guid slotID)
        {
            if (_slots.Remove(slotID))
                SaveSlots();
        }

        public void DeleteAllSlots()
        {
            _slots.Clear();
            PlayerPrefs.DeleteKey(Const.Save.SaveDataPlayerPrefsKey);
            PlayerPrefs.Save();
            _logger.Info(LogTag.Save, "Deleted all save slots.");
        }

        private void SaveSlots()
        {
            var slotsCount = _slots.Count;
            var slotsToSave = new SaveSlotData[slotsCount];

            try
            {
                var index = 0;
                foreach (var (saveSlotID, saveSlotData) in _slots)
                {
                    if (!saveSlotData.IsFresh)
                        slotsToSave[index++] = saveSlotData;
                }

                Array.Resize(ref slotsToSave, index);
                _logger.Info(LogTag.Save, $"Saving {index} save slots.");
            
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
            var skipped = 0;
            for (var i = 0; i < slots.Length; i++)
            {
                var slotData = slots[i];

                // Never-saved slots and slots with lost IDs are leftovers from older builds
                if (slotData == null || slotData.IsFresh || slotData.ID.ToGuid() == Guid.Empty)
                {
                    skipped++;
                    continue;
                }

                _slots[slotData.ID] = slotData;
            }

            if (skipped > 0)
            {
                _logger.Warn(LogTag.Save, $"Skipped {skipped} invalid or never-saved slots. Cleaning them up.");
                SaveSlots();
            }
            
            _logger.Info(LogTag.Save, $"Deserialized {_slots.Count} save slots.");
        }
    }
}