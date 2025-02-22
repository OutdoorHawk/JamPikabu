using System.Collections.Generic;
using Code.Common;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.TextNotification;
using Code.Gameplay.Features.TextNotification.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.Localization;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class WoodChipLootPickupSystem : IExecuteSystem
    {
        private readonly IStaticDataService _staticData;
        private readonly IUIFactory _uiFactory;
        private readonly IWindowService _windowService;
        private readonly INotificationTextService _notificationTextService;
        private readonly ILocalizationService _localizationService;

        private readonly IGroup<GameEntity> _woods;
        private readonly IGroup<GameEntity> _roundTimers;
        private readonly GameContext _context;
        private readonly List<GameEntity> _buffer = new(8);

        public WoodChipLootPickupSystem
        (
            GameContext context,
            IStaticDataService staticData,
            IUIFactory uiFactory,
            IWindowService windowService,
            INotificationTextService notificationTextService,
            ILocalizationService localizationService
        )
        {
            _context = context;
            _staticData = staticData;
            _uiFactory = uiFactory;
            _windowService = windowService;
            _notificationTextService = notificationTextService;
            _localizationService = localizationService;

            _woods = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.WoodChip,
                    GameMatcher.CollectLootRequest,
                    GameMatcher.SpriteRenderer,
                    GameMatcher.TimerRefillAmount
                ));

            _roundTimers = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundInProcess,
                    GameMatcher.RoundTimeLeft
                ));
        }

        public void Execute()
        {
            foreach (var wood in _woods.GetEntities(_buffer))
            {
                wood.isCollectLootRequest = false;
                
                foreach (var timer in _roundTimers)
                    timer.ReplaceRoundTimeLeft(timer.RoundTimeLeft + wood.TimerRefillAmount);

                NotifyText(wood);
                ProcessConsumeVisuals(wood).Forget();
            }
        }

        private void NotifyText(GameEntity wood)
        {
            var notificationTextParameters = new NotificationTextParameters()
            {
                Text = $"+{wood.TimerRefillAmount}{_localizationService["COMMON/TIMER_SECONDS"]}",
                StartPosition = _uiFactory.GetWorldPositionForUI(wood.Transform.position)
            };

            _notificationTextService.ShowNotificationText(notificationTextParameters);
        }

        private async UniTaskVoid ProcessConsumeVisuals(GameEntity wood)
        {
            if (_windowService.TryGetWindow(out PlayerHUDWindow hud) == false)
                return;

            int woodId = wood.Id;

            Vector3 bonfirePos = _uiFactory.GetWorldPositionFromScreenPosition(hud.BonfirePoint.position);
            ChangeLayer(wood);

            wood.isCollected = true;
            wood.isBusy = true;
            wood.Retain(this);

            await FlyAnimation(wood.Transform, bonfirePos);

            GameEntity woodRetained = _context.GetEntityWithId(woodId);

            if (woodRetained.IsNullOrDestructed())
                return;

            woodRetained.Release(this);
            woodRetained.isBusy = false;
            woodRetained.isDestructed = true;
        }

        private void ChangeLayer(GameEntity wood)
        {
            wood.SpriteRenderer.sortingLayerName = "Default";
        }

        private async UniTask FlyAnimation(Transform transform, Vector3 endPos)
        {
            var lootStaticData = _staticData.Get<LootSettingsStaticData>();

            float flyAnimationDuration = lootStaticData.CollectFlyAnimationDuration;
            float jumpPower = Random.Range(-1, 2);

            transform
                .DOScale(0, flyAnimationDuration)
                .SetDelay(flyAnimationDuration)
                .SetLink(transform.gameObject);

            await transform
                    .DOJump(endPos, jumpPower, 1, flyAnimationDuration * 2)
                    .SetLink(transform.gameObject)
                    .AsyncWaitForCompletion()
                ;
        }
    }
}