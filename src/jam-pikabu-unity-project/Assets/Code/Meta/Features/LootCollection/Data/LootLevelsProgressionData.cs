using Code.Gameplay.Features.Loot;

namespace Code.Meta.Features.LootCollection.Data
{
    public struct LootLevelsProgressionData
    {
        public readonly LootTypeId Type;
        public int Level;

        public LootLevelsProgressionData(LootTypeId type, int level)
        {
            Type = type;
            Level = level;
        }
    }
}