using Code.Common.Extensions;
using Code.Meta.Features.Consumables.Service;
using Code.Meta.UI.Shop.Window;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.Consumables.Behaviours
{
    public class ConsumablesShopTabButton : MonoBehaviour
    {
        public GameObject UnlockedButton;
        public ShopTabButton ShopTabButton;
        public Button OpenTabButton;
        
        private IConsumablesUIService _consumablesUIService;

        [Inject]
        private void Construct
        (
            IConsumablesUIService consumablesUIService
        )
        {
            _consumablesUIService = consumablesUIService;
        }

        private void Start()
        {
            RefreshState();
        }

        private void RefreshState()
        {
            if (_consumablesUIService.GetActiveConsumables().Count <= 0)
            {
                ShopTabButton.SetLocked();
                UnlockedButton.DisableElement();
                OpenTabButton.enabled = false;
                return;
            }

            ShopTabButton.SetUnlocked();
            UnlockedButton.EnableElement();
            OpenTabButton.enabled = true;
        }
    }
}