using Code.Common.Extensions;
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

        [Inject]
        private void Construct
        (
            IInstantiator instantiator,
            IStaticDataService staticDataService,
            IShopWindowService shopWindowService
        )
        {
            _instantiator = instantiator;
            _staticData = staticDataService;
            _shopWindowService = shopWindowService;
        }

        public virtual void ActivateTab()
        {
            gameObject.EnableElement();
        }

        public virtual void DeactivateTab()
        {
            gameObject.DisableElement();
        }
    }
}