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
        private readonly IGroup<GameEntity> _loot;
        private readonly List<GameEntity> _lootBuffer = new(64);
        private readonly List<UniTask> _tasksBuffer = new(64);

        public ConsumeLootVisualsSystem(GameContext context, IWindowService windowService, ILootService lootService) : base(context)
        {
            _windowService = windowService;
            _lootService = lootService;
            _loot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Consumed,
                    GameMatcher.LootItemUI
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

            foreach (var loot in _loot)
                loot.Retain(this);

            _windowService.TryGetWindow<PlayerHUDWindow>(out var window);
            var lootContainer = window.GetComponentInChildren<GameplayLootContainer>();

            await ProcessAnimation(lootContainer);

            foreach (var loot in _loot)
                loot.Release(this);

            foreach (var loot in _loot)
                loot.isDestructed = true;

            applier.isDestructed = true;
            _lootService.SetLootIsConsumingState(false);
        }

        private async UniTask ProcessAnimation(GameplayLootContainer lootContainer)
        {
            const float interval = 0.15f;
            _tasksBuffer.Clear();

            foreach (var loot in _loot.GetEntities(_lootBuffer))
            {
                await DelaySeconds(interval, loot.LootItemUI.destroyCancellationToken);

                UniTask task = loot.LootItemUI.AnimateFlyToVat(lootContainer.VatIcon.transform);
                _tasksBuffer.Add(task);
            }

            await UniTask.WhenAll(_tasksBuffer);
        }
    }
}