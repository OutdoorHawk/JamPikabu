using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Project.Code.Editor
{
    public class PrefabListWindow : OdinEditorWindow
    {
        [ListDrawerSettings(DefaultExpandedState = true, DraggableItems = false, HideAddButton = true, HideRemoveButton = true)]
        public List<FolderPrefabList> folderPrefabLists = new List<FolderPrefabList>();

        [FolderPath(AbsolutePath = true)] public string searchFolderPath = "Assets/Prefabs"; // Default search path

        private const string ObstaclesPath = "Assets/Project/Content/Runtime/Obstacles";

        [MenuItem("Tools/Prefab List Window")]
        private static void OpenWindow()
        {
            GetWindow<PrefabListWindow>().Show();
        }

        [Button]
        private void ShowForPath()
        {
            RefreshPrefabLists();
        }

        [Button]
        private void ShowObstacles()
        {
            searchFolderPath = ObstaclesPath;
            RefreshPrefabLists();
        }

        private void RefreshPrefabLists()
        {
            string[] folders = Directory.GetDirectories(searchFolderPath);
            folderPrefabLists.Clear();

            foreach (string folderPath in folders)
            {
                string folderName = CreatePrefabsList(folderPath, out PrefabEditor[] prefabs);
                folderPrefabLists.Add(new FolderPrefabList { FolderName = folderName, PrefabList = prefabs });
            }
        }

        private string CreatePrefabsList(string folderPath, out PrefabEditor[] prefabs)
        {
            string folderName = Path.GetFileName(folderPath);
            GameObject[] prefabList = FindPrefabsInFolder(folderPath);
            prefabs = new PrefabEditor[prefabList.Length];
            for (int i = 0; i < prefabList.Length; i++)
            {
                prefabs[i] = new PrefabEditor
                {
                    PrefabName = prefabList[i].name,
                    Prefab = prefabList[i]
                };
            }

            return folderName;
        }

        private GameObject[] FindPrefabsInFolder(string folderPath)
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

            GameObject[] prefabs = new GameObject[prefabGuids.Length];

            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            }

            return prefabs;
        }

        [System.Serializable]
        public class FolderPrefabList
        {
            [ReadOnly] public string FolderName;
            [ListDrawerSettings(Expanded = true), TableList] public PrefabEditor[] PrefabList;
        }


        [System.Serializable]
        public class PrefabEditor
        {
            [ReadOnly] public string PrefabName;
            [PreviewField] public GameObject Prefab;
        }
    }
}