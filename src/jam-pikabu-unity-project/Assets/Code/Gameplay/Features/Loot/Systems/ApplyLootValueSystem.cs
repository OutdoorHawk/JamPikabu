using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ApplyLootValueSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _roundController;
        private readonly List<GameEntity> _buffer = new(64);
        private readonly IGroup<GameEntity> _loot;

        public ApplyLootValueSystem(GameContext context)
        {
            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected,
                    GameMatcher.ReadyToApply,
                    GameMatcher.GoldValue
                ));
        }

        public void Execute()
        {
            foreach (var loot in _loot.GetEntities(_buffer))
            {
                CreateGameEntity.Empty()
                    .With(x => x.isAddGoldRequest = true)
                    .AddGold(loot.GoldValue.Amount)
                    ;
                
                loot.isReadyToApply = false;
                loot.isApplied = true;
            }
        }
    }
}