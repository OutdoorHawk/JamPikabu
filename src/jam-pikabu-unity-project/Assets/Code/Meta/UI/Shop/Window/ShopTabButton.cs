using Code.Meta.UI.Shop.WindowService;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Shop.Window
{
    public class ShopTabButton : MonoBehaviour
    {
        public ShopTabTypeId TabType;
        public Button Button;

        private IShopWindowService _shopWindowService;

        [Inject]
        private void Construct(IShopWindowService shopWindowService)
        {
            _shopWindowService = shopWindowService;
        }

        private void Start()
        {
            Button.onClick.AddListener(SelectTab);
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(SelectTab);
        }

        private void SelectTab()
        {
            _shopWindowService.SetTabSelected(TabType);
        }
    }
}