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
        
        public void Init(LootSetup setup)
        {
            Icon.sprite = setup.Icon;
            GoldForPickup.SetupPrice(setup.GoldForPicking);
            CanvasGroup.alpha = 0;
        }

        public void Show()
        {
            CanvasGroup
                .DOFade(1, 0.25f)
                .SetLink(gameObject);
        }
    }
}