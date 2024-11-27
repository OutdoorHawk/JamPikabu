using System;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Project.Code.Editor
{
    public class StaticDataEditor : OdinMenuEditorWindow
    {
        private Type _selectedType;

        [MenuItem("Tools/StaticData")]
        private static void OpenWindow()
        {
            GetWindow<StaticDataEditor>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            
            tree.AddAssetAtPath("GameStaticData", "Assets/Resources/GameStaticData.asset");
            tree.AddAllAssetsAtPath("Configs", "Assets/Project/StaticData", true);

            return tree;
        }
    }
}