using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Behaviours;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Configs
{
    public partial class LootSettingsStaticData
    {
        [FoldoutGroup("Editor")] public Transform Parent;

        [FoldoutGroup("Editor")]
        [Button]
        private void CreateLootOnTestScene()
        {
            var parent = Parent;

            foreach (LootItem item in FindObjectsByType<LootItem>(FindObjectsSortMode.None))
                DestroyImmediate(item.gameObject);

            for (int i = 0; i < Configs.Count; i++)
            {
                LootSetup config = Configs[i];
                var loot = Instantiate(LootItem, parent).GetComponent<LootItem>();
                loot.transform.position += loot.transform.position.SetX(1 * i);
                loot.Sprite.sprite = config.Icon;
                loot.Sprite.transform.localScale = Vector3.one * config.Size;
                loot.Sprite.gameObject.AddComponent<PolygonCollider2D>();
            }
        }
    }
}