using System.Threading;
using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Infrastructure.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static System.Threading.CancellationTokenSource;

namespace Code.Gameplay.Features.GrapplingHook.Behaviours
{
    public class GrapplingHookBehaviour : EntityDependant
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _moveAngleDuration = 0.25f;

        private CancellationTokenSource _movementBlendSource;
        private float _lastMoveDirection;

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
    }
}