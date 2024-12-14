using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Customers.Config;
using Code.Gameplay.Features.Customers.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Meta.Features.Days.Service;
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
        [SerializeField] private Animator _bubble;
        [SerializeField] private bool _hideOnAwake;

        private IDaysService _daysService;
        private ICustomersService _customersService;
        private IOrdersService _ordersService;
        private ISoundService _soundService;

        private bool _hided;

        [Inject]
        private void Construct(IDaysService daysService,
            ICustomersService customersService,
            IOrdersService ordersService,
            ISoundService soundService)
        {
            _soundService = soundService;
            _ordersService = ordersService;
            _customersService = customersService;
            _daysService = daysService;
        }

        private void Awake()
        {
            _ordersService.OnOrderUpdated += UpdateCustomer;
            _daysService.OnDayComplete += Hide;
            
            if (_hideOnAwake)
                _hided = true;
        }

        private void Start()
        {
            UpdateSprite();
        }

        private void OnDestroy()
        {
            _ordersService.OnOrderUpdated -= UpdateCustomer;
            _daysService.OnDayComplete -= Hide;
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
            {
                await _bubble.WaitForAnimationCompleteAsync(AnimationParameter.Hide.AsHash(), destroyCancellationToken);
                await _animator.WaitForAnimationCompleteAsync(AnimationParameter.Hide.AsHash(), destroyCancellationToken);
                _soundService.PlayOneShotSound(SoundTypeId.CustomerSwap);
            }

            _hided = false;
            UpdateSprite();
            _soundService.PlayOneShotSound(SoundTypeId.CustomerSwap);
            await _animator.WaitForAnimationCompleteAsync(AnimationParameter.Show.AsHash(), destroyCancellationToken);
            await _bubble.WaitForAnimationCompleteAsync(AnimationParameter.Show.AsHash(), destroyCancellationToken);
        }

        private void UpdateSprite()
        {
            CustomerSetup customer = _customersService.GetCustomerSetup();
            _customerImage.sprite = customer.Sprite;
        }
    }
}