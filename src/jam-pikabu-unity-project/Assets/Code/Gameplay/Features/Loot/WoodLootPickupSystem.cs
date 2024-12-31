using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Loot
{
    public class WoodLootPickupSystem : IExecuteSystem
    {
        private readonly IStaticDataService _staticData;
        private readonly IUIFactory _uiFactory;
        private readonly IWindowService _windowService;

        private readonly IGroup<GameEntity> _woods;
        private readonly IGroup<GameEntity> _roundTimers;

        public WoodLootPickupSystem
        (
            GameContext context,
            IStaticDataService staticData,
            IUIFactory uiFactory,
            IWindowService windowService
        )
        {
            _staticData = staticData;
            _uiFactory = uiFactory;
            _windowService = windowService;
            _woods = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Wood,
                    GameMatcher.CollectLootRequest,
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
            foreach (var wood in _woods)
            {
                foreach (var timer in _roundTimers)
                    timer.ReplaceRoundTimeLeft(timer.RoundTimeLeft + wood.TimerRefillAmount);

                ProcessConsumeVisuals(wood).Forget();
            }
        }

        private async UniTaskVoid ProcessConsumeVisuals(GameEntity wood)
        {
            if (_windowService.TryGetWindow(out PlayerHUDWindow hud) == false)
                return;

            wood.isBusy = true;
            wood.Retain(this);
           // await FlyAnimation(wood.Transform,)
            wood.Release(this);
            wood.isBusy = false;
            wood.isDestructed = true;
        }

        private async UniTask FlyAnimation(Transform transform, Vector3 endPos)
        {
            var lootStaticData = _staticData.Get<LootSettingsStaticData>();

            float flyAnimationDuration = lootStaticData.CollectFlyAnimationDuration;
            float jumpPower = Random.Range(-1, 2);

            await transform
                    .DOJump(endPos, jumpPower, 1, flyAnimationDuration)
                    .SetLink(transform.gameObject)
                    .AsyncWaitForCompletion()
                ;
        }
    }
}