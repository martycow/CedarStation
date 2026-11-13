using Game.General;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class SaveDataMenu
    {
        [MenuItem("Cedar Station/Clear Save Data")]
        private static void ClearSaveData()
        {
            if (!EditorUtility.DisplayDialog(
                    "Clear Save Data",
                    "Delete all Cedar Station save slots from PlayerPrefs?",
                    "Delete",
                    "Cancel"))
                return;

            PlayerPrefs.DeleteKey(Const.Save.SaveDataPlayerPrefsKey);
            PlayerPrefs.Save();
        }
    }
}
