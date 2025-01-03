using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.Gameplay.Features.TextNotification.Behaviours
{
    public class NotificationText : MonoBehaviour
    {
        public TMP_Text _popupText;
        public CanvasGroup _canvasGroup;

        public float fadeDuration = 1f;
        public float moveDistance = 5;
        public float moveDuration = 1f;
        public float startDelay = 0.2f;
        public Ease ease = Ease.OutQuad;

        private void Awake()
        {
            _canvasGroup = _popupText.GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
            {
                _canvasGroup = _popupText.gameObject.AddComponent<CanvasGroup>();
            }
        }

        public void Initialize(in NotificationTextParameters parameters)
        {
            _popupText.text = parameters.Text;
            _canvasGroup.alpha = 1f;

            transform.position = parameters.StartPosition;

            transform
                .DOMoveY(transform.position.y + moveDistance, moveDuration)
                .SetEase(ease)
                .SetDelay(startDelay)
                .SetLink(gameObject);

            _canvasGroup
                .DOFade(0f, fadeDuration)
                .SetEase(ease)
                .SetLink(gameObject)
                .SetDelay(startDelay)
                .OnComplete(() => { Destroy(gameObject); });
        }
    }
}