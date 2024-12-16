using Code.Gameplay.Features.Loot;

namespace Code.Meta.Features.LootCollection
{
    public struct LootItemCollectionData
    {
        public LootTypeId Type;
        public int Level;

        public LootItemCollectionData(LootTypeId type, int level)
        {
            Type = type;
            Level = level;
        }
    }
}