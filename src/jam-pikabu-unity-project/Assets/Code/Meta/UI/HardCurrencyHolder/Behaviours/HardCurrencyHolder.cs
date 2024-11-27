using Code.Gameplay.StaticData;
using Code.Meta.UI.HardCurrencyHolder.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.HardCurrencyHolder.Behaviours
{
    public class HardCurrencyHolder : MonoBehaviour
    {
        [SerializeField] private TMP_Text _amount;
        [SerializeField] private Image _icon;

        private IStorageUIService _storage;
        private IStaticDataService _staticDataService;

        [Inject]
        private void Construct
        (
            IStorageUIService storageUIService,
            IStaticDataService staticDataService
        )
        {
            _staticDataService = staticDataService;
            _storage = storageUIService;
        }

        private void Start()
        {
            _storage.HardChanged += UpdateHard;
            InitIcon();
            UpdateHard();
        }

        private void OnDestroy()
        {
            _storage.HardChanged -= UpdateHard;
        }

        private void InitIcon()
        {
            /*CurrencyConfig config = _staticDataService.GetCurrencyConfig(CurrencyTypeId.Hard);
            _icon.sprite = config.Data.Icon;*/
        }

        private void UpdateHard()
        {
            _amount.text = _storage.CurrentHard.ToString("0");
        }
    }
}