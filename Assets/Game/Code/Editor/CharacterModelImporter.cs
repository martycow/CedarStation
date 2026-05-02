using System.IO;
using UnityEditor;

namespace Editor
{
    public class CharacterModelImporter : AssetPostprocessor
    {
        private const string CharacterPrefix = "Character_";

        void OnPreprocessModel()
        {
            if (!IsCharacterModel(assetPath))
                return;

            var importer = (ModelImporter)assetImporter;

            importer.importAnimation = false;
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

            importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
            importer.materialName = ModelImporterMaterialName.BasedOnMaterialName;
            importer.materialSearch = ModelImporterMaterialSearch.Local;

            importer.SearchAndRemapMaterials(
                ModelImporterMaterialName.BasedOnMaterialName,
                ModelImporterMaterialSearch.Local
            );
        }

        // Validates path matches Assets/Game/<Name>/Models/Character_<Name>.fbx
        private static bool IsCharacterModel(string path)
        {
            if (!path.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                return false;

            var fileName = Path.GetFileNameWithoutExtension(path);
            if (!fileName.StartsWith(CharacterPrefix))
                return false;

            var dir = Path.GetDirectoryName(path)?.Replace('\\', '/') ?? string.Empty;

            // Expect: Assets/Game/<Name>/Models
            var parts = dir.Split('/');
            return parts.Length >= 4
                && parts[0] == "Assets"
                && parts[1] == "Game"
                && parts[3] == "Models";
        }
    }
}
