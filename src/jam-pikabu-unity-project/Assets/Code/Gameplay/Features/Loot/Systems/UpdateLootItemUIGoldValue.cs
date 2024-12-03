using Code.Gameplay.Features.Loot.Behaviours;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class UpdateLootItemUIGoldValue : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _loot;

        public UpdateLootItemUIGoldValue(GameContext context)
        {
            _loot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.GoldValue,
                    GameMatcher.LootItemUI
                ));
        }

        public void Execute()
        {
            foreach (var loot in _loot)
            {
                LootItemUI lootLootItemUI = loot.LootItemUI;
                lootLootItemUI.UpdateValue(loot.GoldValue);
            }
        }
    }
}