using System.Collections.Generic;
using Code.Common;
using Code.Gameplay.Features.Consumables.Behaviours;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Code.Meta.Features.Consumables.Service;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ConsumableLootPickupSystem : IExecuteSystem
    {
        private readonly IConsumablesUIService _consumablesUIService;
        private readonly IWindowService _windowService;
        private readonly IUIFactory _uiFactory;
        private readonly IStaticDataService _staticData;
        private readonly IGroup<GameEntity> _consumableLoot;

        private readonly List<GameEntity> _buffer = new(6);
        private readonly GameContext _context;

        public ConsumableLootPickupSystem
        (
            GameContext context,
            IConsumablesUIService consumablesUIService,
            IWindowService windowService,
            IUIFactory uiFactory,
            IStaticDataService staticData
        )
        {
            _context = context;
            _consumablesUIService = consumablesUIService;
            _windowService = windowService;
            _uiFactory = uiFactory;
            _staticData = staticData;

            _consumableLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.ConsumableTypeId,
                    GameMatcher.CollectLootRequest
                ));
        }

        public void Execute()
        {
            foreach (var entity in _consumableLoot.GetEntities(_buffer))
            {
                entity.isCollectLootRequest = false;
                
                ProcessConsumeVisuals(entity).Forget();
            }
        }

        private async UniTaskVoid ProcessConsumeVisuals(GameEntity consumable)
        {
            if (_windowService.TryGetWindow(out PlayerHUDWindow hud) == false)
                return;

            int woodId = consumable.Id;

            ConsumablesBoostersHolder buttonsHolder = hud.ConsumablesHolder;
            ConsumableBoosterButton button = buttonsHolder.Buttons.Find(button => button.Type == consumable.ConsumableTypeId);

            if (button == null)
            {
                consumable.isDestructed = true;
                return;
            }

            Vector3 endPosition = _uiFactory.GetWorldPositionFromScreenPosition(button.IconBack.transform.position);
            Vector3 endRotation = button.IconBack.transform.rotation.eulerAngles;
            
            ChangeLayer(consumable);

            consumable.isCollected = true;
            consumable.isBusy = true;
            consumable.Retain(this);

            await FlyAnimation(consumable.Transform,consumable.SpriteRenderer, endPosition, endRotation);

            GameEntity retained = _context.GetEntityWithId(woodId);

            if (retained.IsNullOrDestructed())
                return;

            retained.Release(this);
            retained.isBusy = false;
            retained.isDestructed = true;
            _consumablesUIService.AddConsumable(retained.ConsumableTypeId);
        }

        private void ChangeLayer(GameEntity wood)
        {
            wood.SpriteRenderer.sortingLayerName = "Default";
        }

        private async UniTask FlyAnimation(Transform transform, SpriteRenderer renderer, Vector3 endPos, Vector3 endRotation)
        {
            var lootStaticData = _staticData.Get<LootSettingsStaticData>();

            float flyAnimationDuration = lootStaticData.CollectFlyAnimationDuration;
            float jumpPower = Random.Range(-1, 2);

            transform
                .DORotate(endRotation, flyAnimationDuration)
                .SetLink(transform.gameObject);

            transform
                .DOScale(0.5f, flyAnimationDuration)
                .SetDelay(flyAnimationDuration / 2)
                .SetLink(transform.gameObject);
            
            renderer
                .DOFade(0f, flyAnimationDuration)
                .SetDelay(flyAnimationDuration / 2)
                .SetLink(transform.gameObject);

            await transform
                    .DOJump(endPos, jumpPower, 1, flyAnimationDuration)
                    .SetLink(transform.gameObject)
                    .AsyncWaitForCompletion()
                ;
        }
    }
}