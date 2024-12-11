using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Code.Editor
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
            tree.AddAllAssetsAtPath("", "Assets/Project/StaticData", true);

            SortMenuItemsRecursively(tree.MenuItems);
            return tree;
        }

        private void SortMenuItemsRecursively(List<OdinMenuItem> items)
        {
            if (items == null || items.Count == 0)
                return;
            
            items.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            
            foreach (var item in items)
            {
                SortMenuItemsRecursively(item.ChildMenuItems);
            }
        }
    }
}