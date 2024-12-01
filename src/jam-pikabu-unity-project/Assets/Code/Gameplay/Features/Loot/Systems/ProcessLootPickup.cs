using System.Collections.Generic;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Service;
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
    public class ProcessLootPickup : IExecuteSystem, ICleanupSystem
    {
        private readonly IGroup<GameEntity> _loot;
        private readonly ILootUIService _lootUIService;

        private readonly List<GameEntity> _buffer = new(64);
        private readonly IWindowService _windowService;
        private readonly IStaticDataService _staticData;
        private readonly IUIFactory _uiFactory;
        private Camera _camera;

        public ProcessLootPickup(GameContext context, ILootUIService lootUIService, 
            IWindowService windowService, IStaticDataService staticData, IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
            _windowService = windowService;
            _staticData = staticData;
            _lootUIService = lootUIService;
          //  _camera = Camera.main;

            _loot = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Loot,
                    GameMatcher.LootTypeId,
                    GameMatcher.CollectLootRequest
                ));
        }

        public void Execute()
        {
            foreach (var loot in _loot)
            {
                _lootUIService.CreateNewCollectedLootItem(loot.LootTypeId);

                foreach (var collider in loot.Rigidbody2D.GetComponentsInChildren<Collider2D>())
                    collider.enabled = false;

                if (_windowService.TryGetWindow(WindowTypeId.PlayerHUD, out PlayerHUDWindow playerHud) == false)
                    continue;

                var lootContainer = playerHud.GetComponentInChildren<GameplayLootContainer>();
                LootItemUI lootItemUI = lootContainer.Items[^1];
                
                PlayMoveAnimationAsync(lootItemUI, loot).Forget();
            }
        }

        private async UniTaskVoid PlayMoveAnimationAsync(LootItemUI lootItemUI, GameEntity loot)
        {
            _camera = _uiFactory.UIRoot.GetComponent<Canvas>().worldCamera;
            loot.Retain(this);
            
            await UniTask.Yield(lootItemUI.destroyCancellationToken);
            
            var lootStaticData = _staticData.GetStaticData<LootStaticData>();
            
            Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(_camera, lootItemUI.transform.position);

// Устанавливаем Z в фиксированное значение камеры
// Например, используем -10, чтобы совпадать с Z камеры относительно мира
            screenPosition.z = Mathf.Abs(_camera.transform.position.z);

// Конвертируем экранные координаты в мировые
            Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);

// Принудительно фиксируем Z в плоскости мира (например, Z=0 для 2D)
            worldPosition.z = 0;
            
            float flyAnimationDuration = lootStaticData.CollectFlyAnimationDuration;
            float jumpPower = Random.Range(lootStaticData.CollectFlyMinMaxJump.x, lootStaticData.CollectFlyMinMaxJump.y);
            
            loot.Transform
                .DOJump(worldPosition, jumpPower, 1, flyAnimationDuration)
                .SetLink(loot.Transform.gameObject)
                .OnComplete(() => SwapWorldViewToUIView(loot, lootItemUI))
                ;

            loot.Transform
                .DORotate(Vector3.zero, flyAnimationDuration)
                .SetLink(loot.Transform.gameObject)
                ;
        }

        private void SwapWorldViewToUIView(GameEntity loot, LootItemUI lootItemUI)
        {
            loot.Release(this);
            IEntityView lootWorldView = loot.View;
            lootWorldView.ReleaseEntity();
            Object.Destroy(lootWorldView.gameObject);
            loot.RemoveView();
            lootItemUI.EntityView.SetEntity(loot);
            lootItemUI.Show();
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