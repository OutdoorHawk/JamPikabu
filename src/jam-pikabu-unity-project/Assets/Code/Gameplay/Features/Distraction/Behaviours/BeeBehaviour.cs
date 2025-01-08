using System.Collections;
using System.Threading;
using Code.Common.Extensions;
using DG.Tweening;
using UnityEngine;
using static System.Threading.CancellationTokenSource;

namespace Code.Gameplay.Features.Distraction.Behaviours
{
    public class BeeBehaviour : MonoBehaviour
    {
        public Collider2D Collider;
        public Rigidbody2D Rigidbody;

        [Header("Movement Settings")] public float MovementRadius = 4f;
        public float Speed = 6f;
        public float MoveTimeout = 3f;
        public float StopDuration = 1f;

        public float OscillationAmplitude = 0.5f;
        public float OscillationFrequency = 2f;

        private float _moveTimeoutTimer;
        private Vector2 _targetPosition;
        private bool _isMoving = true;
        private Tween _hoverTween;
        private CancellationTokenSource _beeSource;

        private void Start()
        {
            FindNewTargetPosition();
        }

        private void FixedUpdate()
        {
            if (!_isMoving)
                return;
            
            Vector2 currentPosition = Rigidbody.position;

            float oscillation = Mathf.Sin(Time.fixedTime * OscillationFrequency) * OscillationAmplitude;
            Vector2 oscillationOffset = new Vector2(0, oscillation);

            Vector2 nextPosition = Vector2.MoveTowards(currentPosition, _targetPosition, Speed * Time.fixedDeltaTime);
            nextPosition += oscillationOffset;

            Rigidbody.MovePosition(nextPosition);

            if (Vector2.Distance(currentPosition, _targetPosition) < 0.1f)
            {
                StartCoroutine(PauseAndFindNewTarget());
                return;
            }

            if (_moveTimeoutTimer <= 0)
            {
                StartCoroutine(PauseAndFindNewTarget());
                return;
            }
            
            _moveTimeoutTimer -= Time.fixedDeltaTime;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_isMoving)
            {
                StartCoroutine(PauseAndFindNewTarget());
            }
        }

        public void DisableCollider()
        {
            StartCoroutine(DisableColliderRoutine());
        }

        private void FindNewTargetPosition()
        {
            _moveTimeoutTimer = MoveTimeout;
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            _targetPosition = (Vector2)transform.parent.parent.position + randomDirection * Random.Range(0, MovementRadius);
        }

        private IEnumerator PauseAndFindNewTarget()
        {
            _isMoving = false;
            Rigidbody.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(StopDuration);
            FindNewTargetPosition();
            _isMoving = true;
        }

        private IEnumerator DisableColliderRoutine()
        {
            Collider.excludeLayers = CollisionLayer.Hook.AsMask();
            yield return new WaitForSeconds(0.85f);
            Collider.excludeLayers = 0;
        }

        private void OnDestroy()
        {
            _hoverTween?.Kill();
        }
    }
}