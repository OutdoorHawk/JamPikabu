using Code.Infrastructure.Localization;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Settings.Window
{
    public class LocalizationButton : MonoBehaviour
    {
        [SerializeField] private SystemLanguage _type;
        [SerializeField] private Button _button;

        private ILocalizationService _localizationService;

        [Inject]
        private void Construct(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        private void Start()
        {
            _button.onClick.AddListener(UpdateLanguage);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(UpdateLanguage);
        }

        private void UpdateLanguage()
        {
            _localizationService.SetLanguage(_type);
        }
    }
}