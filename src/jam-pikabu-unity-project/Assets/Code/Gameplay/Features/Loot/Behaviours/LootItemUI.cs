using Code.Common;
using Code.Gameplay.Features.Loot.Configs;
using Code.Infrastructure.View;
using Code.Meta.UI.Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Features.Loot.Behaviours
{
    public class LootItemUI : EntityDependant
    {
        public PriceInfo GoldForPickup;
        public Image Icon;
        public CanvasGroup CanvasGroup;
        
        private Tweener _lootItemTween;
        
        public void Init(LootSetup setup)
        {
            Icon.sprite = setup.Icon;
            GoldForPickup.SetupPrice(setup.GoldForPicking);
            CanvasGroup.alpha = 0;
        }

        public void Show()
        {
            _lootItemTween?.Kill();
            _lootItemTween = CanvasGroup
                .DOFade(1, 0.15f)
                .SetLink(gameObject)
                .OnComplete(SetReadyToApply)
                ;
        }

        private void SetReadyToApply()
        {
            _lootItemTween?.Kill();
            _lootItemTween = null;
                    
            if (Entity.IsNullOrDestructed())
                return;

            Entity.isReadyToApply = true;
        }
    }
}