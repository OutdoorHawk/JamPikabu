using System.Collections.Generic;
using System.Threading;
using Code.Common;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Systems
{
    namespace Code.Gameplay.Features.Loot.Systems
    {
        public sealed class LootIngredientPickupVisualSystem : IExecuteSystem
        {
            private readonly GameContext _context;

            private readonly IWindowService _windowService;
            private readonly IStaticDataService _staticData;
            private readonly ISoundService _soundService;
            private readonly IUIFactory _uiFactory;

            private readonly IGroup<GameEntity> _collectedLoot;

            private readonly List<GameEntity> _buffer = new(32);

            public LootIngredientPickupVisualSystem
            (
                GameContext context,
                IWindowService windowService,
                IStaticDataService staticData,
                ISoundService soundService,
                IUIFactory uiFactory
            )
            {
                _context = context;
                _windowService = windowService;
                _staticData = staticData;
                _soundService = soundService;
                _uiFactory = uiFactory;

                _collectedLoot = context.GetGroup(GameMatcher
                    .AllOf(
                        GameMatcher.Loot,
                        GameMatcher.View,
                        GameMatcher.LootTypeId,
                        GameMatcher.Collected,
                        GameMatcher.CollectLootRequest
                    ));
            }

            public void Execute()
            {
                foreach (var loot in _collectedLoot.GetEntities(_buffer))
                {
                    PlayFlyAnimation(loot);
                }

                _buffer.Clear();
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
                    PlayNeutralIngredientFly(loot.Id, playerHud.destroyCancellationToken).Forget();
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
                    .SetLink(loot.Transform.gameObject);

                Vector3 worldPosition = _uiFactory.GetWorldPositionFromScreenPosition(lootItemUI.transform.position);

                await FlyAnimation(loot, worldPosition);

                lootItemUI.AnimateCollected();
                RemoveLootView(loot);

                loot.Release(this);
            }

            private async UniTaskVoid PlayNeutralIngredientFly(int lootId, CancellationToken token)
            {
                await UniTask.Yield(token);

                var loot = _context.GetEntityWithId(lootId);

                if (loot.IsNullOrDestructed())
                    return;
                
                loot.Retain(this);

                if (_windowService.TryGetWindow(WindowTypeId.PlayerHUD, out PlayerHUDWindow hud))
                {
                    Vector3 pos = _uiFactory.GetWorldPositionFromScreenPosition(hud.LootContainer.VatIcon.transform.position);
                    await FlyToVatAnimation(loot, pos);
                }

                RemoveLootView(loot);
                loot.Release(this);
                loot.isDestructed = true;
            }

            private async UniTask FlyAnimation(GameEntity loot, Vector3 targetPos)
            {
                var source = new UniTaskCompletionSource();
                var lootStaticData = _staticData.Get<LootSettingsStaticData>();

                float flyAnimationDuration = lootStaticData.CollectFlyAnimationDuration;
                float jumpPower = Random.Range(-1, 2);

                loot.Transform
                    .DOJump(targetPos, jumpPower, 1, flyAnimationDuration)
                    .SetLink(loot.Transform.gameObject)
                    .OnComplete(() => source.TrySetResult());

                await source.Task;
            }

            private async UniTask FlyToVatAnimation(GameEntity loot, Vector3 targetPos)
            {
                var lootStaticData = _staticData.Get<LootSettingsStaticData>();

                float flyAnimationDuration = lootStaticData.CollectFlyAnimationDuration * 2;
                const float jumpPower = 5f;

                await loot.Transform
                    .DOJump(targetPos, jumpPower, 1, flyAnimationDuration)
                    .SetLink(loot.Transform.gameObject)
                    .AsyncWaitForCompletion();
            }

            private void RemoveLootView(GameEntity loot)
            {
                if (loot.View != null)
                {
                    IEntityView lootWorldView = loot.View;
                    lootWorldView.ReleaseEntity();
                    Object.Destroy(lootWorldView.gameObject);

                    loot.RemoveView();
                    loot.RemoveViewPrefab();
                }

                loot.isBusy = false;
            }
        }
    }
}