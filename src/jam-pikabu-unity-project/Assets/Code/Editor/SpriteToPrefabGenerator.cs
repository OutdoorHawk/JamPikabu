using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Editor
{
    public class SpriteToPrefabGenerator : MonoBehaviour
    {
        [MenuItem("Tools/Generate Prefabs from Sprites")]
        public static void GeneratePrefabs()
        {
            string spritesFolder = EditorUtility.OpenFolderPanel("Select Folder with Sprites", "", "");
            if (string.IsNullOrEmpty(spritesFolder)) return;

            string prefabsFolder = EditorUtility.OpenFolderPanel("Select Folder to Save Prefabs", "", "");
            if (string.IsNullOrEmpty(prefabsFolder)) return;

            // Ensure prefabs folder is relative to the project
            string relativePrefabsFolder = "Assets" + prefabsFolder.Substring(Application.dataPath.Length);
            if (!Directory.Exists(relativePrefabsFolder))
            {
                Directory.CreateDirectory(relativePrefabsFolder);
            }

            // Load all sprites from the selected folder
            string[] spritePaths = Directory.GetFiles(spritesFolder, "*.png", SearchOption.AllDirectories);
            foreach (string spritePath in spritePaths)
            {
                string relativeSpritePath = "Assets" + spritePath.Substring(Application.dataPath.Length);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(relativeSpritePath);

                if (sprite != null)
                {
                    CreatePrefabFromSprite(sprite, relativePrefabsFolder);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Prefabs generated successfully.");
        }

        private static void CreatePrefabFromSprite(Sprite sprite, string prefabsFolder)
        {
            // Create root GameObject
            GameObject root = new GameObject(sprite.name);
            root.AddComponent<RectTransform>();

            // Create child GameObject with Image component
            GameObject child = new GameObject("Image");
            child.transform.SetParent(root.transform);

            Image image = child.AddComponent<Image>();
            image.sprite = sprite;
            image.SetNativeSize();

            // Save prefab
            string prefabPath = Path.Combine(prefabsFolder, sprite.name + ".prefab");
            prefabPath = prefabPath.Replace("\\", "/");
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);

            // Clean up temporary GameObject
            GameObject.DestroyImmediate(root);
        }
    }
}