using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot;

namespace Code.Meta.Features.LootCollection.Factory
{
    public class LootCollectionFactory : ILootCollectionFactory
    {
        public void UnlockNewLoot(LootTypeId type)
        {
            CreateMetaEntity.Empty()
                .With(x => x.isLoot = true)
                .AddLootTypeId(type)
                .AddLevel(0)
                ;
        }
    }
}