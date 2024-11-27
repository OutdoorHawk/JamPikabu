using Code.Common.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.Gameplay.Tutorial.Window
{
    public class TutorialMessageBox : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TutorialMessageAnchorType _anchorType;

        public TutorialMessageAnchorType AnchorType => _anchorType;

        private Tweener _tweener;
        
        private const float ANIM_DURATION = 0.15f;
        
        private static readonly Vector3 AnimScale = new(0.75f, 0.75f, 0.75f);

        public void Show(string text)
        {
            if (string.IsNullOrEmpty(_text.text))
                _text.text = text;

            gameObject.SetActive(true);
            PlayShowTween(text);
        }

        private void PlayShowTween(string text)
        {
            _tweener?.Kill();
            _tweener = transform
                    .DOScale(AnimScale, ANIM_DURATION)
                    .SetLink(gameObject)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => UpdateTextAndPlayBack(text))
                ;
        }

        private void UpdateTextAndPlayBack(string text)
        {
            if (this == null)
                return;

            _text.text = text;
            _tweener?.Kill();
            _tweener = transform
                    .DOScale(Vector3.one, ANIM_DURATION)
                    .SetEase(Ease.OutQuad)
                    .SetLink(gameObject)
                ;
        }

        public void PlayHide()
        {
            _tweener?.Kill();
            _tweener = transform
                .DOScale(Vector3.zero, ANIM_DURATION)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject)
                .OnComplete(() => gameObject.DisableSafe())
                ;
        }

        private void OnDestroy()
        {
            _tweener?.Kill();
        }
    }
}