using System.Collections.Generic;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.View;
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
        private readonly Camera _camera;

        public ProcessLootPickup(GameContext context, ILootUIService lootUIService, IWindowService windowService)
        {
            _windowService = windowService;
            _lootUIService = lootUIService;
            _camera = Camera.main;

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

                Vector3 screenPosition = lootItemUI.transform.position;
                Vector3 worldPosition = _camera.ScreenToWorldPoint(new Vector2(screenPosition.x, screenPosition.y));

                loot.Retain(this);
                loot.Transform
                    .DOJump(worldPosition, Random.Range(-1, 2), 1, 0.25f)
                    .SetLink(loot.Transform.gameObject)
                    .OnComplete(() => SwapWorldViewToUIView(loot, lootItemUI))
                    ;
            }
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