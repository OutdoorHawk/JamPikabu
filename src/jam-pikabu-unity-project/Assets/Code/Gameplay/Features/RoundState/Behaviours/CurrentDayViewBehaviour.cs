using System;
using Code.Meta.Features.Days.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.RoundState.Behaviours
{
    public class CurrentDayViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dayText;

        private IDaysService _daysService;

        [Inject]
        private void Construct(IDaysService daysService)
        {
            _daysService = daysService;
        }

        private void Start()
        {
            _daysService.OnDayBegin += UpdateText;
            UpdateText();
        }

        private void OnDestroy()
        {
            _daysService.OnDayBegin -= UpdateText;
        }

        private void UpdateText()
        {
            _dayText.text = $"{_daysService.CurrentDay}/{_daysService.MaxDays}";
        }
    }
}