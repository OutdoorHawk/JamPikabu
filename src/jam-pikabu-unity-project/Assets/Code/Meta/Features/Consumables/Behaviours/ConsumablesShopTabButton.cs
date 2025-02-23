using Code.Common.Extensions;
using Code.Meta.Features.Consumables.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.Consumables.Behaviours
{
    public class ConsumablesShopTabButton : MonoBehaviour
    {
        public GameObject LockedButton;
        public GameObject UnlockedButton;
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
                LockedButton.EnableElement();
                UnlockedButton.DisableElement();
                OpenTabButton.enabled = false;
                return;
            }

            LockedButton.DisableElement();
            UnlockedButton.EnableElement();
            OpenTabButton.enabled = true;
        }
    }
}