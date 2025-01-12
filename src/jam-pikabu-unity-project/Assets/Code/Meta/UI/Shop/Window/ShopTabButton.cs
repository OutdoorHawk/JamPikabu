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
        public GameObject OpenedTabIcon;
        public GameObject ClosedTabIcon;

        private IShopWindowService _shopWindowService;

        [Inject]
        private void Construct(IShopWindowService shopWindowService)
        {
            _shopWindowService = shopWindowService;
        }

        private void Start()
        {
            Button.onClick.AddListener(SelectTab);
            _shopWindowService.OnSelectionChanged += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(SelectTab);
            _shopWindowService.OnSelectionChanged -= Refresh;
        }

        private void SelectTab()
        {
            _shopWindowService.SetTabSelected(TabType);
        }

        private void Refresh()
        {
            OpenedTabIcon.SetActive(_shopWindowService.SelectedTab == TabType);
            ClosedTabIcon.SetActive(_shopWindowService.SelectedTab != TabType);
        }
    }
}