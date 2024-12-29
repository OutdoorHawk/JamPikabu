using System.Collections.Generic;
using Code.Common;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class LootWithRatingPickupSystem : IExecuteSystem, ICleanupSystem
    {
        private readonly GameContext _context;
        private readonly IGroup<GameEntity> _loot;
        private readonly IGameplayLootService _gameplayLootService;

        private readonly List<GameEntity> _buffer = new(64);
        private readonly IWindowService _windowService;
        private readonly IStaticDataService _staticData;
        private readonly ISoundService _soundService;
        private readonly IOrdersService _ordersService;
        private readonly Camera _camera;
        private readonly IGroup<GameEntity> _timers;

        public LootWithRatingPickupSystem(GameContext context, IGameplayLootService gameplayLootService,
            IWindowService windowService, IStaticDataService staticData, ISoundService soundService, IOrdersService ordersService)
        {
            _context = context;
            _windowService = windowService;
            _staticData = staticData;
            _soundService = soundService;
            _ordersService = ordersService;
            _gameplayLootService = gameplayLootService;
            _camera = Camera.main;
            
           _timers = context.GetGroup(GameMatcher.AllOf(
                GameMatcher.RoundInProcess,
                GameMatcher.RoundTimeLeft));


            _loot = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Loot,
                    GameMatcher.LootTypeId,
                    GameMatcher.CollectLootRequest,
                    GameMatcher.Rating
                ));
        }

        public void Execute()
        {
            foreach (var loot in _loot)
            {
                SetLootCollected(loot);

                TryCompleteTimerIfAllLootCollected();

                DisableCollisions(loot);

                PlayFlyAnimation(loot);
            }
        }

        private void SetLootCollected(GameEntity loot)
        {
            _gameplayLootService.AddCollectedLoot(loot.LootTypeId);
            loot.isCollected = true;
            loot.isBusy = true;
        }

        private void TryCompleteTimerIfAllLootCollected()
        {
            if (_ordersService.GetOrderProgress() >= 1)
            {
                foreach (var timer in _timers)
                {
                    timer.ReplaceRoundTimeLeft(0);
                }
            }
        }

        private static void DisableCollisions(GameEntity loot)
        {
            foreach (var collider in loot.Rigidbody2D.GetComponentsInChildren<Collider2D>())
                collider.enabled = false;
        }

        private void PlayFlyAnimation(GameEntity loot)
        {
            if (_windowService.TryGetWindow(WindowTypeId.PlayerHUD, out PlayerHUDWindow playerHud) == false)
                return;

            var lootContainer = playerHud.GetComponentInChildren<GameplayLootContainer>();

            if (lootContainer.ItemsByLootType.TryGetValue(loot.LootTypeId, out LootItemUI lootItemUI))
            {
                _soundService.PlaySound(SoundTypeId.LootFly);
                PlayMoveAnimationAsync(lootItemUI, loot.Id).Forget();
            }
            else
            {
                _soundService.PlaySound(SoundTypeId.LootFly);
                PlayNeutralIngredientFly(loot).Forget();
            }
        }

        private async UniTaskVoid PlayMoveAnimationAsync(LootItemUI lootItemUI, int lootId)
        {
            await UniTask.Yield(lootItemUI.destroyCancellationToken);

            var loot = _context.GetEntityWithId(lootId);

            if (loot.IsNullOrDestructed())
                return;

            loot.Retain(this);
            var lootStaticData = _staticData.Get<LootSettingsStaticData>();
            
            loot.Transform
                .DORotate(Vector3.zero, lootStaticData.CollectFlyAnimationDuration)
                .SetLink(loot.Transform.gameObject)
                ;

            Vector3 worldPosition = GetWorldPositionFromScreenPosition(lootItemUI.transform.position);

            await FlyAnimation(loot, worldPosition);
            
            loot.Release(this);
            RemoveLootView(loot);
            lootItemUI.AnimateCollected();
        }

        private async UniTaskVoid PlayNeutralIngredientFly(GameEntity loot)
        {
            loot.Retain(this);

            _windowService.TryGetWindow(out PlayerHUDWindow hud);
           
            Vector3 pos2 = GetWorldPositionFromScreenPosition(hud.LootContainer.VatIcon.transform.position);
            
            await FlyToVatAnimation(loot, pos2);

            RemoveLootView(loot);
            loot.Release(this);
        }

        private async UniTask FlyAnimation(GameEntity loot, Vector3 pos1)
        {
            UniTaskCompletionSource source = new UniTaskCompletionSource();
            var lootStaticData = _staticData.Get<LootSettingsStaticData>();

            float flyAnimationDuration = lootStaticData.CollectFlyAnimationDuration;
            float jumpPower = Random.Range(-1, 2);

            loot.Transform
                .DOJump(pos1, jumpPower, 1, flyAnimationDuration)
                .SetLink(loot.Transform.gameObject)
                .OnComplete(() => source.TrySetResult())
                ;
            
            await source.Task;
        }
        
        private async UniTask FlyToVatAnimation(GameEntity loot, Vector3 pos1)
        {
            var lootStaticData = _staticData.Get<LootSettingsStaticData>();

            float flyAnimationDuration = lootStaticData.CollectFlyAnimationDuration;
            const float jumpPower = 5;

           await loot.Transform
                .DOJump(pos1, jumpPower, 1, flyAnimationDuration * 2)
                .SetLink(loot.Transform.gameObject)
                .AsyncWaitForCompletion()
                ;
        }

        private Vector3 GetWorldPositionFromScreenPosition(Vector3 screenPos)
        {
            Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(_camera, screenPos);
            screenPosition.z = Mathf.Abs(_camera.transform.position.z);
            Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;
            return worldPosition;
        }

        private void RemoveLootView(GameEntity loot)
        {
            IEntityView lootWorldView = loot.View;
            lootWorldView.ReleaseEntity();
            Object.Destroy(lootWorldView.gameObject);
            loot.RemoveView();
            loot.RemoveViewPrefab();
            loot.isBusy = false;
        }

        public void Cleanup()
        {
            foreach (var loot in _loot.GetEntities(_buffer))
            {
                loot.isCollectLootRequest = false;
            }
        }
    }
}