using Code.Gameplay.Features.Loot;

namespace Code.Meta.Features.LootCollection.Factory
{
    public interface ILootCollectionFactory
    {
        MetaEntity UnlockNewLoot(LootTypeId type);
    }
}