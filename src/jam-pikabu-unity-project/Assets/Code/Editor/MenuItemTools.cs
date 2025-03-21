using Code.Infrastructure.SceneLoading;
using Code.Progress;
using Code.Progress.SaveLoadService;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Code.Editor
{
    public class MenuItemTools : EditorWindow
    {
        [MenuItem("Tools/Scenes/Open Bootstrap Scene", false, 1)]
        public static void OpenBootstrapScene()
        {
            EditorSceneManager.OpenScene($"Assets/Project/Scenes/{SceneTypeId.BootstrapScene}.unity", OpenSceneMode.Single);
        }

        [MenuItem("Tools/Scenes/Open Menu Scene", false, 2)]
        public static void OpenMenuScene()
        {
            EditorSceneManager.OpenScene($"Assets/Project/Scenes/{SceneTypeId.MapMenu}.unity", OpenSceneMode.Single);
        }
        
        [MenuItem("Tools/Scenes/Open Gameplay Scene", false, 2)]
        public static void OpenGameplayScene()
        {
            EditorSceneManager.OpenScene($"Assets/Project/Scenes/Gameplay/{SceneTypeId.DefaultGameplayScene}.unity", OpenSceneMode.Single);
        }
        
        [MenuItem("Tools/Scenes/Open Loot Setup", false, 2)]
        public static void OpenLootSetupScene()
        {
            EditorSceneManager.OpenScene($"Assets/Project/Scenes/Editor/LootSetup.unity", OpenSceneMode.Single);
        }

        [MenuItem("Tools/PlayerProgress/DeleteProgress")]
        public static void ClearPlayerProgress()
        {
            ProgressExtensions.DeleteProgress(SaveLoadService.PlayerProgressPath);
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Tools/System/Recompile Code")]
        public static void Recompile()
        {
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
    }
}