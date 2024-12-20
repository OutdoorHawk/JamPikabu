using Code.Gameplay.Features.Loot;

namespace Code.Meta.Features.LootCollection.Factory
{
    public interface ILootCollectionFactory
    {
        MetaEntity CreateNewLootProgressionEntity(LootTypeId type);
        MetaEntity CreateLootFreeUpgradeTimer(LootTypeId type, int timer);
    }
}