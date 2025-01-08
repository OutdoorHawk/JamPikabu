using System.Threading;
using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities.Config;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static System.Threading.CancellationTokenSource;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Abilities.Behaviours
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class StickToKinematic : MonoBehaviour
    {
        public LayerMask StickLayerMask;

        private IStaticDataService _staticData;
        private ISoundService _soundService;
        
        private Rigidbody2D _rb;
        private Transform _originParent;
        private CancellationTokenSource _stickSource;

        private float _stickDuration;

        [Inject]
        private void Construct(IStaticDataService staticData, ISoundService soundService)
        {
            _soundService = soundService;
            _staticData = staticData;
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _originParent = transform.parent;
            AbilityData abilityData = _staticData.Get<AbilityStaticData>().GetDataByType(AbilityTypeId.StickyToHook);
            _stickDuration = abilityData.Value;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.layer.Matches(StickLayerMask))
                return;

            var otherRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (otherRb == null || otherRb.bodyType != RigidbodyType2D.Kinematic)
                return;

            ActivateSticky(collision).Forget();
        }

        private async UniTaskVoid ActivateSticky(Collision2D collision)
        {
            if (_stickSource != null)
                return;
            
            ResetToken();
            EnableSticky(collision);
            await DelaySeconds(_stickDuration, _stickSource.Token);
            DisableSticky();
            _stickSource = null;
        }

        private void ResetToken()
        {
            _stickSource?.Cancel();
            _stickSource = CreateLinkedTokenSource(destroyCancellationToken);
        }

        private void EnableSticky(Collision2D collision)
        {
            transform.SetParent(collision.collider.transform);
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _soundService.PlaySound(SoundTypeId.StickyAbility);
        }

        private void DisableSticky()
        {
            transform.SetParent(_originParent);
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }
}