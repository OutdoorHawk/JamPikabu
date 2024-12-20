using Code.Gameplay.Features.Loot;

namespace Code.Meta.Features.LootCollection.Data
{
    public struct LootFreeUpgradeTimerData
    {
        public readonly LootTypeId Type;
        public int NextFreeUpgradeTimeStamp;

        public LootFreeUpgradeTimerData(LootTypeId type, int nextFreeUpgradeTimeStamp)
        {
            Type = type;
            NextFreeUpgradeTimeStamp = nextFreeUpgradeTimeStamp;
        }
    }
}