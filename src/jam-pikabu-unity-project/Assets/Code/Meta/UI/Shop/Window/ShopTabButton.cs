using Code.Common.Extensions;
using Code.Meta.UI.Shop.WindowService;
using Code.Progress.Provider;
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
        public GameObject Pin;

        private IShopWindowService _shopWindowService;
        private IProgressProvider _progress;

        [Inject]
        private void Construct(IShopWindowService shopWindowService, IProgressProvider progress)
        {
            _progress = progress;
            _shopWindowService = shopWindowService;
        }

        private void Start()
        {
            Button.onClick.AddListener(SelectTab);
            _shopWindowService.OnSelectionChanged += Refresh;
            Refresh();
            ValidatePin();
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(SelectTab);
            _shopWindowService.OnSelectionChanged -= Refresh;
        }

        private void SelectTab()
        {
            _shopWindowService.SetTabSelected(TabType);
            ValidatePin();
        }

        private void Refresh()
        {
            OpenedTabIcon.SetActive(_shopWindowService.SelectedTab == TabType);
            ClosedTabIcon.SetActive(_shopWindowService.SelectedTab != TabType);
        }

        private void ValidatePin()
        {
            Pin.DisableSafe();
            
            if (TabType == ShopTabTypeId.LootUpgrade)
                return;
            
            if (_progress.Progress.Tutorial.ConsumablesPinSeen)
                return;
            
            if (_shopWindowService.SelectedTab == ShopTabTypeId.Consumables)
            {
                _progress.Progress.Tutorial.ConsumablesPinSeen = true;
                return;
            }
            
            Pin.EnableSafe();
        }
    }
}