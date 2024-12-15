using System.Collections.Generic;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Windows.Service;
using Cysharp.Threading.Tasks;
using Entitas;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ConsumeLootVisualsSystem : ReactiveSystem<GameEntity>
    {
        private readonly IWindowService _windowService;
        private readonly ILootService _lootService;
        private readonly IGroup<GameEntity> _consumedLoot;

        public ConsumeLootVisualsSystem(GameContext context, IWindowService windowService, ILootService lootService) : base(context)
        {
            _windowService = windowService;
            _lootService = lootService;
            _consumedLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Consumed
                ));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.LootEffectsApplier,
                GameMatcher.Available).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isLootEffectsApplier && entity.isAvailable;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var applier in entities)
            {
                applier.isAvailable = false;
                AnimateAsync(applier).Forget();
            }
        }

        private async UniTaskVoid AnimateAsync(GameEntity applier)
        {
            _lootService.SetLootIsConsumingState(true);
            
            _windowService.TryGetWindow<PlayerHUDWindow>(out var hud);
            var lootContainer = hud.GetComponentInChildren<GameplayLootContainer>();
            var orderView = hud.OrderViewBehaviour;
            
            await lootContainer.AnimateFlyToVat(_consumedLoot);
            orderView.InitOrderFillProgress();

            foreach (var loot in _consumedLoot)
                loot.isDestructed = true;

            applier.isDestructed = true;
            _lootService.SetLootIsConsumingState(false);
        }
    }
}