using Code.Gameplay.Features.Loot;

namespace Code.Meta.Features.LootCollection.Factory
{
    public interface ILootCollectionFactory
    {
        void UnlockNewLoot(LootTypeId type);
    }
}