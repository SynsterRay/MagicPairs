using UnityEngine;
using UnityEditor;

namespace MagicPairs.Editor
{
    public static class UIButtonsImporter
    {
        [MenuItem("MagicPairs/Fix UI Button Imports")]
        public static void FixImports()
        {
            int count = FixFolder("Assets/Resources/UIButtons", 256);
            Debug.Log($"[UIButtonsImporter] Fixed {count} UI button textures.");
        }

        [MenuItem("MagicPairs/Fix Card Imports")]
        public static void FixCardImports()
        {
            int total = 0;
            total += FixFolder("Assets/Resources/CarCards", 512);
            total += FixFolder("Assets/Resources/WaterWorldCards", 512);
            total += FixFolder("Assets/Resources/PrincessCards", 512);
            total += FixFolder("Assets/Resources/AnimalCards", 512);
            total += FixFolder("Assets/Resources/SpaceAnimalsCards", 512);
            total += FixFolder("Assets/Resources/Backgrounds", 1024);
            Debug.Log($"[CardImporter] Fixed {total} card textures.");
        }

        private static int FixFolder(string folder, int maxSize = 2048)
        {
            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
            int count = 0;

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool changed = false;

                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    changed = true;
                }
                if (importer.spriteImportMode == SpriteImportMode.None)
                {
                    importer.spriteImportMode = SpriteImportMode.Single;
                    changed = true;
                }
                if (importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = false;
                    changed = true;
                }
                if (!importer.alphaIsTransparency)
                {
                    importer.alphaIsTransparency = true;
                    changed = true;
                }

                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                if (settings.spriteMeshType != SpriteMeshType.FullRect)
                {
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    importer.SetTextureSettings(settings);
                    changed = true;
                }

                // Keep developer logo at full resolution
                bool isLogo = path.Contains("developer_logo");
                int effectiveMax = isLogo ? 2048 : maxSize;

                if (importer.maxTextureSize > effectiveMax)
                {
                    importer.maxTextureSize = effectiveMax;
                    changed = true;
                }

                if (changed)
                {
                    importer.SaveAndReimport();
                    count++;
                }
            }

            return count;
        }
    }
}
