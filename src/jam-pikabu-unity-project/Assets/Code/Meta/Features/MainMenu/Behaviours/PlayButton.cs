using Code.Common.Extensions;
using Code.Infrastructure.SceneLoading;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.MainMenu.Service;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Code.Meta.Features.MainMenu.Behaviours
{
    public class PlayButton : MonoBehaviour
    {
        public CanvasGroup BossContent;
        public CanvasGroup SpecialContent;

        private IMapMenuService _mapMenuService;
        private IDaysService _daysService;

        [Inject]
        private void Construct
        (
            IMapMenuService mapMenuService,
            IDaysService daysService
        )
        {
            _daysService = daysService;
            _mapMenuService = mapMenuService;
        }

        private void Start()
        {
            _mapMenuService.OnSelectionChanged += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            _mapMenuService.OnSelectionChanged -= Refresh;
        }

        private void Refresh()
        {
            DayData dayData = _daysService.GetDayData(_mapMenuService.SelectedDayId);

            if (dayData.IsBossDay)
            {
                BossContent.EnableElement();
                BossContent.alpha = 1;
                return;
            }
            
            HideElement(BossContent);

            if (dayData.SceneId != SceneTypeId.DefaultGameplayScene)
            {
                SpecialContent.EnableElement();
                SpecialContent.alpha = 1;
                return;
            }
            
            HideElement(SpecialContent);
        }

        private void ResetAll()
        {
            HideElement(BossContent);
            HideElement(SpecialContent);
        }

        private void HideElement(CanvasGroup element)
        {
            if (element.alpha < 1)
                return;

            element.DOFade(0, 0.15f)
                .SetLink(gameObject)
                .OnComplete(element.DisableElement)
                ;
        }
    }
}