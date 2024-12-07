using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Customers.Config;
using Code.Gameplay.Features.Customers.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Customers.Behaviours
{
    public class CustomerBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _customerImage;
        [SerializeField] private Animator _animator;
        [SerializeField] private bool _hideOnAwake;

        private IRoundStateService _roundStateService;
        private ICustomersService _customersService;
        private IOrdersService _ordersService;

        private bool _hided;

        [Inject]
        private void Construct(IRoundStateService roundStateService, ICustomersService customersService, IOrdersService ordersService)
        {
            _ordersService = ordersService;
            _customersService = customersService;
            _roundStateService = roundStateService;
        }

        private void Awake()
        {
            _ordersService.OnOrderUpdated += UpdateCustomer;
            _roundStateService.OnDayComplete += Hide;

            UpdateSprite();

            if (_hideOnAwake) 
                _hided = true;
        }

        private void OnDestroy()
        {
            _ordersService.OnOrderUpdated -= UpdateCustomer;
            _roundStateService.OnDayComplete -= Hide;
        }

        private void UpdateCustomer()
        {
            UpdateAsync().Forget();
        }

        private void Hide()
        {
            _animator.SetTrigger(AnimationParameter.Hide.AsHash());
        }

        private async UniTaskVoid UpdateAsync()
        {
            if (_hided == false) 
                await _animator.WaitForAnimationCompleteAsync(AnimationParameter.Hide.AsHash(), destroyCancellationToken);
            
            UpdateSprite();
            _animator.SetTrigger(AnimationParameter.Show.AsHash());
            _hided = false;
        }

        private void UpdateSprite()
        {
            CustomerSetup customer = _customersService.GetCustomerSetup();
            _customerImage.sprite = customer.Sprite;
        }
    }
}