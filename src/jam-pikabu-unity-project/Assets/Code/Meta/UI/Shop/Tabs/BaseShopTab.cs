using Code.Common.Extensions;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Meta.UI.Shop.Window;
using Code.Meta.UI.Shop.WindowService;
using UnityEngine;
using Zenject;

namespace Code.Meta.UI.Shop.Tabs
{
    public class BaseShopTab : MonoBehaviour
    {
        public ShopTabTypeId TypeId;

        protected IInstantiator _instantiator;
        protected IStaticDataService _staticData;
        protected IShopWindowService _shopWindowService;
        protected ISoundService _soundService;

        [Inject]
        private void Construct
        (
            IInstantiator instantiator,
            IStaticDataService staticDataService,
            IShopWindowService shopWindowService,
            ISoundService soundService
        )
        {
            _soundService = soundService;
            _instantiator = instantiator;
            _staticData = staticDataService;
            _shopWindowService = shopWindowService;
        }

        public virtual void ActivateTab()
        {
            gameObject.EnableElement();
            _soundService.PlaySound(SoundTypeId.OpenShop);
        }

        public virtual void DeactivateTab()
        {
            gameObject.DisableElement();
        }
    }
}