using System.Threading;
using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Infrastructure.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;
using static System.Threading.CancellationTokenSource;
using static Code.Common.Extensions.AsyncGameplayExtensions;
using Random = UnityEngine.Random;

namespace Code.Gameplay.Features.GrapplingHook.Behaviours
{
    public class GrapplingHookBehaviour : EntityDependant
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _hookCenter;
        [SerializeField] private GameObject _heavyParticles;
        [SerializeField] private float _moveAngleDuration = 0.25f;
        [SerializeField] private float _ascentDelay = 0.25f;
        [SerializeField] private float _openClawsDelay = 0.2f;
        [SerializeField] private float _collectAnimationDelay = 1f;
        [SerializeField] private Color _slowerColor;
        [SerializeField] private Color _fasterColor;
        [SerializeField] private LayerMask _triggerMask;

        private ISoundService _soundService;
        private CancellationTokenSource _movementBlendSource;
        private CancellationTokenSource _speedModificationSource;

        private float _lastMoveDirection;

        private Tween[] _colorTweens;
        private Material[] _materials;

        public bool Triggered { get; private set; }

        public Transform HookCenter => _hookCenter;

        [Inject]
        private void Construct(ISoundService soundService)
        {
            _soundService = soundService;
        }

        private void Start()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            _materials = new Material[renderers.Length];
            _colorTweens = new Tween[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
                _materials[i] = renderers[i].material;
            
            _heavyParticles.DisableElement();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer.Matches(_triggerMask) == false)
                return;

            if (other.isTrigger)
                return;
            
            Triggered = true;
        }

        public void SetupXMovement(float moveDirection)
        {
            if (Mathf.Approximately(_lastMoveDirection, moveDirection))
                return;

            _movementBlendSource?.Cancel();
            _movementBlendSource = CreateLinkedTokenSource(destroyCancellationToken);

            _animator.ToFloatParameter
            (
                AnimationParameter.MovementBlend.AsHash(),
                moveDirection,
                _moveAngleDuration,
                _movementBlendSource.Token
            ).Forget();

            _lastMoveDirection = moveDirection;
        }

        public void OpenClaws()
        {
            _animator.SetBool(AnimationParameter.Open.AsHash(), true);
        }

        public void CloseClawsAndReturn()
        {
            CloseAndAscentAsync().Forget();
        }

        public void StartDescending()
        {
            if (Entity.isDescending)
                return;

            if (Entity.isClosingClaws)
                return;

            Triggered = false;
            _soundService.PlayOneShotSound(SoundTypeId.HookDescend);
        }

        public void ApplySpeedChange(float factor, float duration, int speedUpChance)
        {
            ApplySpeedChangeAsync(factor, duration, speedUpChance).Forget();
        }

        public void ShowHeavyParticles()
        {
            if (_heavyParticles.activeSelf)
                return;
            
            ShowHeavyParticlesAsync().Forget();
        }

        private async UniTaskVoid CloseAndAscentAsync()
        {
            Entity.isClosingClaws = true;

            _animator.SetBool(AnimationParameter.Open.AsHash(), false);

            await DelaySeconds(_ascentDelay, destroyCancellationToken);

            if (Entity != null)
                Entity.isAscentRequested = true;

            if (Entity != null)
                Entity.isClosingClaws = false;
        }

        private async UniTaskVoid ApplySpeedChangeAsync(float factor, float duration, int speedUpChance)
        {
            _speedModificationSource?.Cancel();
            _speedModificationSource = CreateLinkedTokenSource(destroyCancellationToken);

            bool speedUp = speedUpChance > Random.Range(0, 101);

            if (speedUp)
            {
                ApplyColor(_slowerColor, 0.25f);
                factor *= -1;
                _soundService.PlaySound(SoundTypeId.FreezeHookAbility);
            }
            else
            {
                ApplyColor(_fasterColor, 0.25f);
                _soundService.PlaySound(SoundTypeId.BoostHookAbility);
            }

            Entity.ReplaceHookSpeedModifier(1 + factor);

            await DelaySeconds(duration, _speedModificationSource.Token);

            Entity.ReplaceHookSpeedModifier(1);
            ApplyColor(Color.white, 0.25f);
        }

        private async UniTaskVoid ShowHeavyParticlesAsync()
        {
            _heavyParticles.EnableElement();
            await DelaySeconds(1, destroyCancellationToken);
            _heavyParticles.DisableElement();
        }

        private void ApplyColor(Color newColor, float duration)
        {
            for (int i = 0; i < _materials.Length; i++)
            {
                Material material = _materials[i];

                _colorTweens[i]?.Kill();
                _colorTweens[i] = material
                        .DOColor(newColor, duration)
                        .SetLink(gameObject)
                    ;
            }
        }
    }
}